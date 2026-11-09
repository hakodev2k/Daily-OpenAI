public readonly record struct FeedBatch(string Id, int BagCount, int GramsPerBag);
public static class FeedBatchCalculator
{
    public static long TotalGrams(FeedBatch batch)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(batch.BagCount);
        ArgumentOutOfRangeException.ThrowIfNegative(batch.GramsPerBag);
        long total = batch.BagCount * batch.GramsPerBag;
        return total;
    }
}
public static class Program
{
    private static readonly (FeedBatch Batch, long Expected)[] Fixtures =
    {
        (new("BATCH-101", 100, 25_000), 2_500_000L),
        (new("BATCH-900", 120_000, 25_000), 3_000_000_000L),
        (new("BATCH-MAX", int.MaxValue, int.MaxValue), 4_611_686_014_132_420_609L)
    };
    public static int Main(string[] args)
    {
        string mode = args.FirstOrDefault() ?? "--demo";
        if (mode is not ("--demo" or "--reproduce" or "--verify")) return 2;
        int failures = 0;
        foreach (var (batch, expected) in Fixtures)
        {
            try
            {
                long actual = FeedBatchCalculator.TotalGrams(batch);
                bool match = actual == expected;
                Console.WriteLine($"{batch.Id}: count={batch.BagCount} gramsPerBag={batch.GramsPerBag} actual={actual} expected={expected} match={match}");
                if (!match) failures++;
            }
            catch (Exception ex) { Console.WriteLine($"{batch.Id}: exception={ex.GetType().Name}"); failures++; }
        }
        if (mode == "--demo") return 0;
        if (mode == "--reproduce")
        {
            if (failures == 0) { Console.Error.WriteLine("REPRODUCE FAILED"); return 1; }
            Console.WriteLine($"REPRODUCED: {failures} mismatches"); return 0;
        }
        if (failures > 0) { Console.Error.WriteLine($"VERIFY FAILED: {failures} mismatches"); return 1; }
        Console.WriteLine("VERIFY PASS"); return 0;
    }
}
