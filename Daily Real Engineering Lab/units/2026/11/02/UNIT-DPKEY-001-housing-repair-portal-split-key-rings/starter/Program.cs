using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

internal static class Program
{
    private const string Payload = "REQ-2048|operator-17";
    private const string ApplicationName = "housing-repair-portal";
    private const string Purpose = "session-ticket/v1";

    private static int Main(string[] args)
    {
        if (args.Length > 1) return Usage();
        string mode = args.Length == 0 ? "--run" : args[0];
        if (mode is not ("--run" or "--reproduce" or "--verify")) return Usage();

        string root = Path.Combine(Path.GetTempPath(), "dpkey-lab-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var report = RunScenario(root);
            Console.WriteLine($"local={report.Local}, crossReplica={report.CrossReplica}, " +
                $"restart={report.Restart}, otherAppRejected={report.OtherAppRejected}, " +
                $"tamperRejected={report.TamperRejected}");

            if (mode == "--run") return 0;
            if (mode == "--reproduce")
            {
                bool reproduced = report.Local && !report.CrossReplica &&
                    report.Restart && report.OtherAppRejected && report.TamperRejected;
                Console.WriteLine(reproduced ? "REPRODUCED: replica-dependent session failure." :
                    "NOT REPRODUCED: expected starter symptom absent.");
                return reproduced ? 0 : 1;
            }

            bool verified = report.Local && report.CrossReplica &&
                report.Restart && report.OtherAppRejected && report.TamperRejected;
            Console.WriteLine(verified ? "VERIFIED: session compatibility and isolation contracts pass." :
                "FAILED: session compatibility or isolation contract violated.");
            return verified ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            return 3;
        }
        finally
        {
            try { Directory.Delete(root, recursive: true); }
            catch (IOException) { /* Temporary files are isolated per run. */ }
            catch (UnauthorizedAccessException) { /* Best-effort cleanup on Windows. */ }
        }
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
        return 2;
    }

    private static Report RunScenario(string root)
    {
        var nodeA = CreateProvider(root, "node-a", ApplicationName);
        var ticketA = nodeA.CreateProtector(Purpose);
        string protectedTicket = ticketA.Protect(Payload);

        bool local = CanRead(ticketA, protectedTicket);
        var nodeB = CreateProvider(root, "node-b", ApplicationName);
        bool crossReplica = CanRead(nodeB.CreateProtector(Purpose), protectedTicket);

        var restartedA = CreateProvider(root, "node-a", ApplicationName);
        bool restart = CanRead(restartedA.CreateProtector(Purpose), protectedTicket);

        var unrelatedApp = CreateProvider(root, "node-b", "housing-repair-admin");
        bool otherAppRejected = !CanRead(unrelatedApp.CreateProtector(Purpose), protectedTicket);

        char replacement = protectedTicket[0] == 'A' ? 'B' : 'A';
        string tampered = replacement + protectedTicket[1..];
        bool tamperRejected = !CanRead(ticketA, tampered);
        return new Report(local, crossReplica, restart, otherAppRejected, tamperRejected);
    }

    private static IDataProtectionProvider CreateProvider(
        string root, string node, string applicationName)
    {
        string folder = Path.Combine(root, node);
        Directory.CreateDirectory(folder);
        return DataProtectionProvider.Create(
            new DirectoryInfo(folder),
            builder => builder.SetApplicationName(applicationName));
    }

    private static bool CanRead(IDataProtector protector, string protectedTicket)
    {
        try
        {
            return string.Equals(protector.Unprotect(protectedTicket),
                Payload, StringComparison.Ordinal);
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

    private sealed record Report(bool Local, bool CrossReplica, bool Restart,
        bool OtherAppRejected, bool TamperRejected);
}
