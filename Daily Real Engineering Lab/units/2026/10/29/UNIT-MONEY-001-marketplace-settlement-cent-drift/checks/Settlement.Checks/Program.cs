using Settlement.Core;

internal static class Program
{
    private const decimal Rate = 0.10m;

    private static int Main(string[] args)
    {
        var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
        return mode switch
        {
            "demo" => Demo(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => Unknown(mode)
        };
    }

    private static LedgerEntry[] SmallSales(string seller, int count)
        => Enumerable.Range(1, count)
            .Select(i => new LedgerEntry(seller, seller + "-sale-" + i, 0.05m))
            .ToArray();

    private static int Demo()
    {
        var batch = SmallSales("ART-17", 3)
            .Concat(new[] { new LedgerEntry("ART-21", "refund-1", -0.05m) });
        foreach (var statement in SettlementCalculator.Build(batch, Rate))
            Console.WriteLine($"{statement.SellerId}: gross={statement.Gross:F2}, fee={statement.Fee:F2}, net={statement.Net:F2}");
        return 0;
    }

    private static int Reproduce()
    {
        var actual = SettlementCalculator.Build(SmallSales("ART-17", 3), Rate).Single();
        Console.WriteLine($"Observed: gross={actual.Gross:F2}, fee={actual.Fee:F2}, net={actual.Net:F2}");
        Console.WriteLine("Contract ledger: gross=0.15, fee=0.03, net=0.12");
        if (actual.Fee == 0.02m && actual.Net == 0.13m)
        {
            Console.WriteLine("REPRODUCED: settlement differs from posted transaction fees.");
            return 0;
        }
        Console.Error.WriteLine("Expected starter symptom was not reproduced. Inspect source or prior edits.");
        return 1;
    }

    private static int Verify()
    {
        var failures = 0;
        Check("three-small-sales", SmallSales("A", 3),
            new[] { new SellerStatement("A", 0.15m, 0.03m, 0.12m) }, ref failures);
        Check("three-small-refunds", Enumerable.Range(1, 3)
                .Select(i => new LedgerEntry("R", "refund-" + i, -0.05m)).ToArray(),
            new[] { new SellerStatement("R", -0.15m, -0.03m, -0.12m) }, ref failures);
        Check("mixed-sellers", SmallSales("A", 3).Concat(SmallSales("B", 2)).ToArray(),
            new[] { new SellerStatement("A", 0.15m, 0.03m, 0.12m),
                    new SellerStatement("B", 0.10m, 0.02m, 0.08m) }, ref failures);
        Check("sale-and-refund", new[] {
                new LedgerEntry("C", "s1", 0.05m),
                new LedgerEntry("C", "s2", 0.05m),
                new LedgerEntry("C", "r1", -0.05m) },
            new[] { new SellerStatement("C", 0.05m, 0.01m, 0.04m) }, ref failures);
        Check("ordinary-sale", new[] { new LedgerEntry("D", "s1", 100.00m) },
            new[] { new SellerStatement("D", 100.00m, 10.00m, 90.00m) }, ref failures);

        try
        {
            _ = SettlementCalculator.Build(Array.Empty<LedgerEntry>(), 1.01m);
            Console.Error.WriteLine("FAIL invalid-rate: expected ArgumentOutOfRangeException");
            failures++;
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("PASS invalid-rate");
        }

        Console.WriteLine(failures == 0
            ? "VERIFIED: all settlement invariants passed."
            : $"VERIFICATION FAILED: {failures} contract(s) violated.");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(
        string name, IReadOnlyList<LedgerEntry> entries,
        IReadOnlyList<SellerStatement> expected, ref int failures)
    {
        var actual = SettlementCalculator.Build(entries, Rate);
        var ok = actual.SequenceEqual(expected);
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name}");
        if (ok) return;
        failures++;
        Console.WriteLine("  Expected: " + string.Join("; ", expected));
        Console.WriteLine("  Actual:   " + string.Join("; ", actual));
    }

    private static int Unknown(string mode)
    {
        Console.Error.WriteLine($"Unknown mode: {mode}. Use demo, reproduce, verify.");
        return 2;
    }
}
