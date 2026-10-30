namespace LaundryAudit;

public readonly record struct LaundryScan(string BagId, bool IsReadable);

public interface IAcceptedScanCounter
{
    int Value { get; }
    void Increment();
}

public sealed class AcceptedScanCounter : IAcceptedScanCounter
{
    private int _value;
    public int Value => _value;
    public void Increment() => _value++;
}

public sealed class ShiftScanner
{
    private readonly IAcceptedScanCounter _counter;

    public ShiftScanner(IAcceptedScanCounter counter)
    {
        _counter = counter;
    }

    public int Process(IEnumerable<LaundryScan> scans)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var accepted = 0;

        foreach (var scan in scans)
        {
            if (!scan.IsReadable || string.IsNullOrWhiteSpace(scan.BagId) ||
                !seen.Add(scan.BagId))
            {
                continue;
            }

            _counter.Increment();
            accepted++;
        }

        return accepted;
    }

    public int WorkerCount => _counter.Value;
}

public readonly record struct ShiftSummary(
    int AcceptedByScanner,
    int WorkerCount,
    int ReportCount);

public static class ShiftService
{
    public static ShiftSummary CloseShift(IEnumerable<LaundryScan> scans)
    {
        var counter = new AcceptedScanCounter();
        var scanner = new ShiftScanner(counter);
        var accepted = scanner.Process(scans);
        return new ShiftSummary(accepted, scanner.WorkerCount, counter.Value);
    }
}
