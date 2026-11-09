namespace RescueLab;

public sealed record DispatchTicket(string IncidentId, int Severity);

public sealed class DispatchQueue
{
    private readonly Dictionary<string, int> _current = new(StringComparer.Ordinal);
    private readonly PriorityQueue<string, int> _pending = new();

    public void Enqueue(string id, int severity)
    {
        Validate(severity);
        if (!_current.TryAdd(id, severity))
            throw new InvalidOperationException("Duplicate incident");
        _pending.Enqueue(id, severity);
    }

    public void Revise(string id, int severity)
    {
        Validate(severity);
        if (!_current.ContainsKey(id))
            throw new KeyNotFoundException(id);
        _current[id] = severity;
    }

    public DispatchTicket? DispatchNext()
    {
        if (!_pending.TryDequeue(out var id, out _))
            return null;
        var severity = _current[id];
        _current.Remove(id);
        return new DispatchTicket(id, severity);
    }

    public int PendingCount => _current.Count;

    private static void Validate(int severity)
    {
        if (severity is < 1 or > 9)
            throw new ArgumentOutOfRangeException(nameof(severity));
    }
}
