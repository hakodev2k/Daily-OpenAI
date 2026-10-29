namespace FerryDispatchLab;
public sealed record DispatchJob(string JobId, int Priority, long ReceivedSequence);
public sealed class DispatchQueue
{
    private static readonly IComparer<DispatchJob> Order = Comparer<DispatchJob>.Create(
        (left, right) =>
        {
            int priority = right.Priority.CompareTo(left.Priority);
            if (priority != 0) return priority;
            int arrival = left.ReceivedSequence.CompareTo(right.ReceivedSequence);
            if (arrival != 0) return arrival;
            return StringComparer.Ordinal.Compare(left.JobId, right.JobId);
        });
    private readonly SortedSet<DispatchJob> _pending = new(Order);
    public bool Enqueue(DispatchJob job) => _pending.Add(job);
    public IReadOnlyList<DispatchJob> Snapshot() => _pending.ToArray();
}
