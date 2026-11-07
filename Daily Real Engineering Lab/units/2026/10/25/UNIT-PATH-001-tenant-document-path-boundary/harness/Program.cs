using System;
using System.IO;

namespace PathBoundaryLab;

internal static class Program
{
    private sealed record ProbeResult(string Name, string Status, string? Content);

    public static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "--inspect" : args[0];
        if (mode is not ("--inspect" or "--reproduce" or "--verify"))
        {
            Console.Error.WriteLine("Usage: --inspect | --reproduce | --verify");
            return 2;
        }

        string sandbox = Path.Combine(Path.GetTempPath(), "path-boundary-lab-" + Guid.NewGuid().ToString("N"));
        try
        {
            string tenantRoot = Path.Combine(sandbox, "tenant-42");
            string authorizedFolder = Path.Combine(tenantRoot, "reports", "2026");
            string similarSibling = Path.Combine(sandbox, "tenant-42-backup");
            string unrelatedSibling = Path.Combine(sandbox, "unrelated");

            Directory.CreateDirectory(authorizedFolder);
            Directory.CreateDirectory(similarSibling);
            Directory.CreateDirectory(unrelatedSibling);
            File.WriteAllText(Path.Combine(authorizedFolder, "summary.txt"), "approved");
            File.WriteAllText(Path.Combine(similarSibling, "confidential.txt"), "restricted");
            File.WriteAllText(Path.Combine(unrelatedSibling, "note.txt"), "other");

            ProbeResult valid = Probe("valid", tenantRoot,
                Path.Combine("reports", "2026", "summary.txt"));
            ProbeResult similar = Probe("similar-sibling", tenantRoot,
                Path.Combine("..", "tenant-42-backup", "confidential.txt"));
            ProbeResult unrelated = Probe("unrelated-sibling", tenantRoot,
                Path.Combine("..", "unrelated", "note.txt"));

            foreach (ProbeResult item in new[] { valid, similar, unrelated })
                Console.WriteLine($"CASE {item.Name}: {item.Status}; content={item.Content ?? "<none>"}");

            bool validAllowed = valid.Status == "ALLOWED" && valid.Content == "approved";
            bool similarLeaked = similar.Status == "ALLOWED" && similar.Content == "restricted";
            bool similarDenied = similar.Status == "DENIED";
            bool unrelatedDenied = unrelated.Status == "DENIED";

            bool success = mode switch
            {
                "--reproduce" => validAllowed && similarLeaked && unrelatedDenied,
                "--verify" => validAllowed && similarDenied && unrelatedDenied,
                _ => true
            };

            Console.WriteLine($"MODE {mode}: {(success ? "PASS" : "FAIL")}");
            return success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"UNEXPECTED {ex.GetType().Name}: {ex.Message}");
            return 2;
        }
        finally
        {
            if (Directory.Exists(sandbox))
            {
                try { Directory.Delete(sandbox, recursive: true); }
                catch (IOException) { Console.Error.WriteLine("WARN: temporary fixture cleanup failed."); }
                catch (UnauthorizedAccessException) { Console.Error.WriteLine("WARN: fixture cleanup was denied."); }
            }
        }
    }

    private static ProbeResult Probe(string name, string tenantRoot, string requestedPath)
    {
        try
        {
            string resolved = DocumentPathPolicy.ResolveDocumentPath(tenantRoot, requestedPath);
            return new ProbeResult(name, "ALLOWED", File.ReadAllText(resolved));
        }
        catch (UnauthorizedAccessException)
        {
            return new ProbeResult(name, "DENIED", null);
        }
    }
}
