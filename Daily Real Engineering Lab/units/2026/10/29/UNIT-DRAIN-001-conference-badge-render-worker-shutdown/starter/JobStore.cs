namespace BadgeDrainLab;

internal readonly record struct JobSnapshot(
    int Pending, int InFlight, int Completed, int Artifacts, int PublishAttempts);

internal sealed class JobStore
{
    private readonly Queue<string> _pending = new();
    private readonly HashSet<string> _inFlight = new(StringComparer.Ordinal);
    private readonly HashSet<string> _completed = new(StringComparer.Ordinal);
    private readonly HashSet<string> _artifacts = new(StringComparer.Ordinal);

    public JobStore(string initialJob) => _pending.Enqueue(initialJob);
    public int PublishAttempts { get; private set; }

    public string? Lease()
    {
        if (_pending.Count == 0) return null;
        var id = _pending.Dequeue();
        if (!_inFlight.Add(id)) throw new InvalidOperationException("Duplicate active lease.");
        return id;
    }

    public void Acknowledge(string id)
    {
        if (!_inFlight.Remove(id)) throw new InvalidOperationException("Ack without active lease.");
        _completed.Add(id);
    }

    public void Release(string id)
    {
        if (!_inFlight.Remove(id)) throw new InvalidOperationException("Release without active lease.");
        _pending.Enqueue(id);
    }

    public void Publish(string id)
    {
        PublishAttempts++;
        _artifacts.Add(id); // One artifact key per badge; repeated upload is idempotent.
    }

    public JobSnapshot Snapshot() => new(
        _pending.Count, _inFlight.Count, _completed.Count,
        _artifacts.Count, PublishAttempts);
}
