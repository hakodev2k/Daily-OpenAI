using System.Collections.Concurrent;
using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
{
    Console.Error.WriteLine("Usage: dotnet run -- --check");
    return 2;
}
return await Scenarios.RunAsync() ? 0 : 1;

public sealed record Attachment(int Id, string FileName);
public sealed record BatchReport(int Completed, int Peak, int Throttled, bool AllClean, long ElapsedMs);

public sealed class PartnerScanner
{
    private int _active, _peak, _throttled;
    public PartnerScanner(int capacity)
    {
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity));
        Capacity = capacity;
    }
    public int Capacity { get; }
    public int Peak => Volatile.Read(ref _peak);
    public int Throttled => Volatile.Read(ref _throttled);

    public async Task<bool> ScanAsync(Attachment attachment, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        int active = Interlocked.Increment(ref _active);
        while (true)
        {
            int old = Volatile.Read(ref _peak);
            if (active <= old || Interlocked.CompareExchange(ref _peak, active, old) == old)
                break;
        }
        try
        {
            if (active > Capacity)
            {
                Interlocked.Increment(ref _throttled);
                return false; // Fake HTTP 429.
            }
            await Task.Delay(TimeSpan.FromMilliseconds(80), token);
            return true;
        }
        finally { Interlocked.Decrement(ref _active); }
    }
}

public static class AttachmentBatch
{
    public static async Task<BatchReport> ScanAsync(
        IReadOnlyList<Attachment> attachments, PartnerScanner scanner, CancellationToken token)
    {
        var results = new ConcurrentBag<bool>();
        var timer = Stopwatch.StartNew();
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Min(6, Math.Max(1, scanner.Capacity - 1)),
            CancellationToken = token
        };
        await Parallel.ForEachAsync(attachments, options, async (file, ct) =>
        {
            results.Add(await scanner.ScanAsync(file, ct));
        });
        timer.Stop();
        return new BatchReport(results.Count, scanner.Peak, scanner.Throttled,
            results.All(x => x), timer.ElapsedMilliseconds);
    }
}

public static class Scenarios
{
    public static async Task<bool> RunAsync()
    {
        bool a = await RunCaseAsync(8, 4, 8);
        bool b = await RunCaseAsync(1, 1, 1);
        return a && b;
    }
    private static async Task<bool> RunCaseAsync(int capacity, int minPeak, int maxPeak)
    {
        var files = Enumerable.Range(1, 24)
            .Select(i => new Attachment(i, $"application-{i:D3}.pdf")).ToArray();
        var scanner = new PartnerScanner(capacity);
        var report = await AttachmentBatch.ScanAsync(files, scanner, CancellationToken.None);
        bool pass = report.Completed == 24 && report.AllClean && report.Throttled == 0 &&
                    report.Peak >= minPeak && report.Peak <= maxPeak;
        Console.WriteLine($"CAPACITY_{capacity}={(pass ? "PASS" : "FAIL")} " +
            $"completed={report.Completed} peak={report.Peak} throttled={report.Throttled} " +
            $"allClean={report.AllClean} elapsedMs={report.ElapsedMs}");
        return pass;
    }
}
