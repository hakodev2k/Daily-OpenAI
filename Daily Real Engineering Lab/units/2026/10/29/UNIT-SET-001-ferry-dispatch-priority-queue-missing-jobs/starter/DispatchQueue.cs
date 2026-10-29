namespace FerryDispatchLab;
public sealed record DispatchJob(string JobId, int Priority, long ReceivedSequence);
public sealed class DispatchQueue
{
    private readonly SortedSet<DispatchJob> _pending = new(
        Comparer<DispatchJob>.Create((left, right) =>
            right.Priority.CompareTo(left.Priority)));
    public bool Enqueue(DispatchJob job) => _pending.Add(job);
    public IReadOnlyList<DispatchJob> Snapshot() => _pending.ToArray();
}
