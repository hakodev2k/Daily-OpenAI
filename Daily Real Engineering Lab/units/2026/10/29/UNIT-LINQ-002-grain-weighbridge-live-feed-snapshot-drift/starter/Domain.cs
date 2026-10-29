namespace WeighbridgeLab;

public sealed record Receipt(string ReceiptId, decimal WeightKg);
public sealed record IntakeReport(int ReceiptCount, decimal TotalWeightKg)
{
    public decimal AverageWeightKg => ReceiptCount == 0 ? 0m : TotalWeightKg / ReceiptCount;
}

public interface IReceiptFeed
{
    IEnumerable<Receipt> ReadCurrent();
}

// Mỗi lần một consumer mở feed, gateway cung cấp snapshot mới nhất.
public sealed class RotatingReceiptFeed : IReceiptFeed
{
    private readonly IReadOnlyList<Receipt>[] _snapshots;
    private int _opens;

    public RotatingReceiptFeed(params IReadOnlyList<Receipt>[] snapshots)
    {
        if (snapshots.Length == 0) throw new ArgumentException("At least one snapshot is required.", nameof(snapshots));
        _snapshots = snapshots;
    }

    public int SnapshotOpens => Volatile.Read(ref _opens);

    public IEnumerable<Receipt> ReadCurrent()
    {
        var opened = Interlocked.Increment(ref _opens);
        var snapshot = _snapshots[Math.Min(opened - 1, _snapshots.Length - 1)];
        foreach (var receipt in snapshot)
            yield return receipt;
    }
}
