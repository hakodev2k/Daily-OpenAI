using System.Diagnostics;
using ColdChainReceiptLab;

return await LabRunner.MainAsync(args);

internal static class LabRunner
{
    public static async Task<int> MainAsync(string[] args)
    {
        var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
        if (mode is not ("demo" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project starter -- [demo|reproduce|verify]");
            return 2;
        }

        int count = mode == "verify" ? 16 : 1;
        bool anyUnexpected = false;
        for (int i = 0; i < count; i++)
        {
            var observation = await ObserveAsync(i);
            Console.WriteLine(
                $"case={i} accepted={observation.Accepted} receipt={observation.ReceiptId} " +
                $"auditCount={observation.AuditCount} auditInsideGate={observation.AuditInsideGate} " +
                $"producerElapsedMs={observation.ProducerElapsedMs:F1}");

            bool contractOk = observation.Accepted &&
                observation.ReceiptId == $"PAL-{i:000}" &&
                observation.AuditCount == 1;

            if (!contractOk || (mode == "verify" && observation.AuditInsideGate))
                anyUnexpected = true;

            if (mode == "reproduce" && !observation.AuditInsideGate)
                anyUnexpected = true;
        }

        if (mode == "demo")
            return anyUnexpected ? 1 : 0;

        if (anyUnexpected)
        {
            Console.Error.WriteLine(mode == "verify"
                ? "FAIL: audit executed in producer critical section or business contract regressed."
                : "FAIL: baseline symptom not observed; inspect environment or starter changes.");
            return 1;
        }

        Console.WriteLine(mode == "verify"
            ? "PASS: all receipts matched; audit once; no audit inside producer critical section."
            : "REPRODUCED: audit ran while producer held the critical section.");
        return 0;
    }

    private static async Task<Observation> ObserveAsync(int index)
    {
        var coordinator = new ReceiptCoordinator();
        var id = $"PAL-{index:000}";
        coordinator.Track(id);
        bool auditInsideGate = false;
        int auditCount = 0;

        // Register the awaiting continuation before delivering the acknowledgement.
        var awaiting = coordinator.WaitAndAuditAsync(id, _ =>
        {
            auditInsideGate = coordinator.IsGateEnteredByCurrentThread;
            Thread.Sleep(60); // Local deterministic stand-in for a slow legacy audit adapter.
            Interlocked.Increment(ref auditCount);
        });

        var sw = Stopwatch.StartNew();
        bool accepted = coordinator.Accept(new PalletReceipt(id, "Accepted"));
        sw.Stop();

        var receipt = await awaiting.ConfigureAwait(false);
        return new Observation(accepted, receipt.PalletId, auditCount,
            auditInsideGate, sw.Elapsed.TotalMilliseconds);
    }

    private sealed record Observation(
        bool Accepted, string ReceiptId, int AuditCount,
        bool AuditInsideGate, double ProducerElapsedMs);
}
