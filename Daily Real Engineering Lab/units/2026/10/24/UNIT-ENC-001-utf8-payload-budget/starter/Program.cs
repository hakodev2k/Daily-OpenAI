using System.Text;

namespace EncodingLab;

internal static class Program
{
    private const int Budget = 16;

    private sealed record Sample(string Name, string Text, bool ExpectedAllowed);

    private static readonly Sample[] Samples =
    [
        new("ascii-exact-boundary", new string('A', 16), true),
        new("ascii-over-boundary", new string('A', 17), false),
        new("vietnamese-exact-boundary", string.Concat(Enumerable.Repeat("á", 8)), true),
        new("vietnamese-over-boundary", string.Concat(Enumerable.Repeat("á", 9)), false),
        new("emoji-exact-boundary", string.Concat(Enumerable.Repeat("📦", 4)), true),
        new("emoji-over-boundary", string.Concat(Enumerable.Repeat("📦", 5)), false),
        new("order-notice", string.Concat(Enumerable.Repeat("Đơn", 5)), false),
        new("empty", string.Empty, true)
    ];

    private static int Main(string[] args)
    {
        var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
        return mode switch
        {
            "run" => RunProbe(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => PrintUsage()
        };
    }

    private static int PrintUsage()
    {
        Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
        return 2;
    }

    private static int RunProbe()
    {
        Console.WriteLine("case | localAllowed | providerStatus | utf16Units | wireBytes");
        foreach (var sample in Samples)
        {
            var allowed = PayloadPolicy.CanSend(sample.Text, Budget);
            var status = allowed ? ProviderSimulator.Send(sample.Text, Budget).ToString() : "LOCAL_BLOCK";
            Console.WriteLine($"{sample.Name} | {allowed} | {status} | {sample.Text.Length} | {Encoding.UTF8.GetByteCount(sample.Text)}");
        }
        return 0;
    }

    private static int Reproduce()
    {
        var affected = Samples.Where(x => x.Name is "vietnamese-over-boundary" or "emoji-over-boundary" or "order-notice").ToArray();
        var failures = 0;
        foreach (var sample in affected)
        {
            var allowed = PayloadPolicy.CanSend(sample.Text, Budget);
            var status = allowed ? ProviderSimulator.Send(sample.Text, Budget) : 0;
            Console.WriteLine($"{sample.Name}: localAllowed={allowed}, providerStatus={(allowed ? status.ToString() : "LOCAL_BLOCK")}, utf16Units={sample.Text.Length}, wireBytes={Encoding.UTF8.GetByteCount(sample.Text)}");
            if (!allowed || status != 413) failures++;
        }
        if (!PayloadPolicy.CanSend(new string('A', Budget), Budget) ||
            ProviderSimulator.Send(new string('A', Budget), Budget) != 202) failures++;
        Console.WriteLine(failures == 0 ? "REPRODUCED: 3 partner rejections after local acceptance." : $"NOT REPRODUCED: {failures} checks differ from original symptom.");
        return failures == 0 ? 0 : 1;
    }

    private static int Verify()
    {
        var failures = 0;
        foreach (var sample in Samples)
        {
            var allowed = PayloadPolicy.CanSend(sample.Text, Budget);
            var actualStatus = allowed ? ProviderSimulator.Send(sample.Text, Budget) : 0;
            var expectedStatus = sample.ExpectedAllowed ? 202 : 0;
            var passed = allowed == sample.ExpectedAllowed && actualStatus == expectedStatus;
            Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {sample.Name}: allowed={allowed}, status={(allowed ? actualStatus.ToString() : "LOCAL_BLOCK")}, expectedAllowed={sample.ExpectedAllowed}");
            if (!passed) failures++;
        }
        try
        {
            _ = PayloadPolicy.CanSend(null!, Budget);
            Console.WriteLine("FAIL null-input contract");
            failures++;
        }
        catch (ArgumentNullException) { Console.WriteLine("PASS null-input contract"); }
        try
        {
            _ = PayloadPolicy.CanSend("ok", -1);
            Console.WriteLine("FAIL negative-budget contract");
            failures++;
        }
        catch (ArgumentOutOfRangeException) { Console.WriteLine("PASS negative-budget contract"); }
        Console.WriteLine(failures == 0 ? "VERIFIED: all boundary, Unicode and argument checks passed." : $"NOT VERIFIED: {failures} failed checks.");
        return failures == 0 ? 0 : 1;
    }
}

internal static class ProviderSimulator
{
    // Provider's public contract: the UTF-8 encoded message body may not exceed maxBytes.
    public static int Send(string message, int maxBytes)
    {
        return Encoding.UTF8.GetByteCount(message) <= maxBytes ? 202 : 413;
    }
}
