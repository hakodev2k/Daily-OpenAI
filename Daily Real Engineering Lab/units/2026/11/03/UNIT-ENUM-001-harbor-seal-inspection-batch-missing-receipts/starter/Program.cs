using System.Collections;
using SealImportLab;

return LabRunner.Run(args);

internal static class LabRunner
{
    public static int Run(string[] args)
    {
        try
        {
            switch (args.FirstOrDefault())
            {
                case "--reproduce":
                    Reproduce();
                    return 0;
                case "--verify":
                    Verify();
                    return 0;
                default:
                    Console.Error.WriteLine("Usage: dotnet run -- --reproduce | --verify");
                    return 2;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAILED: {ex.Message}");
            return 1;
        }
    }

    private static SealScan[] Fixture() =>
    [
        new("S-101", "GATE-A", true),
        new("S-102", "GATE-A", true),
        new("S-103", "GATE-B", false)
    ];

    private static void Reproduce()
    {
        var feed = new OneShotFeed(Fixture());
        var stored = new List<SealScan>();
        var report = new ScanImportService().Import(feed, stored);
        Console.WriteLine($"received={report.Received} stored={report.Stored} rejected={report.Rejected} source_remaining={feed.Remaining} enumerations={feed.EnumerationCount}");
        Check(report == new ImportReport(3, 0, 0)
            && stored.Count == 0 && feed.Remaining == 0,
            "Triệu chứng gốc không còn đúng. Nếu đã sửa starter, hãy chạy --verify.");
        Console.WriteLine("REPRODUCED: số lượng nhận được không khớp với phiếu đã xử lý.");
    }

    private static void Verify()
    {
        var feed = new OneShotFeed(Fixture());
        var stored = new List<SealScan>();
        var report = new ScanImportService().Import(feed, stored);
        Check(report == new ImportReport(3, 2, 1), $"one-shot report sai: {report}");
        Check(stored.Select(s => s.SealCode).SequenceEqual(["S-101", "S-102"]),
            "phiếu lưu không đúng thứ tự hoặc thiếu dữ liệu");
        Check(feed.Remaining == 0 && feed.EnumerationCount == 1,
            $"nguồn one-shot phải được duyệt đúng một lượt, actual={feed.EnumerationCount}");

        var replayableStored = new List<SealScan>();
        var replayable = new ScanImportService().Import(Fixture(), replayableStored);
        Check(replayable == new ImportReport(3, 2, 1) && replayableStored.Count == 2,
            "regression: nguồn replayable");

        var empty = new OneShotFeed([]);
        var emptyStored = new List<SealScan>();
        Check(new ScanImportService().Import(empty, emptyStored) == new ImportReport(0, 0, 0),
            "regression: batch rỗng");
        Check(empty.EnumerationCount == 1 && emptyStored.Count == 0,
            "regression: batch rỗng vẫn phải có một lượt đọc");

        var invalid = new OneShotFeed(
        [
            new("", "GATE-C", true),
            new("S-999", "GATE-C", false)
        ]);
        var invalidStored = new List<SealScan>();
        Check(new ScanImportService().Import(invalid, invalidStored) == new ImportReport(2, 0, 2)
            && invalidStored.Count == 0 && invalid.EnumerationCount == 1,
            "regression: scan không đạt kiểm tra");

        Console.WriteLine("PASS: one-shot, replayable, empty, rejected và single-pass contract.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

internal sealed class OneShotFeed : IEnumerable<SealScan>
{
    private readonly Queue<SealScan> _pending;

    public OneShotFeed(IEnumerable<SealScan> scans) =>
        _pending = new Queue<SealScan>(scans);

    public int Remaining => _pending.Count;
    public int EnumerationCount { get; private set; }

    public IEnumerator<SealScan> GetEnumerator()
    {
        EnumerationCount++;
        while (_pending.Count > 0)
            yield return _pending.Dequeue();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
