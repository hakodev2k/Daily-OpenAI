namespace RescueLab;

public sealed record DispatchTicket(string IncidentId, int Severity);

public sealed class DispatchQueue
{
    private sealed record Entry(string Id, int Severity, long Version);
    private readonly Dictionary<string, (int Severity, long Version)> _current = new(StringComparer.Ordinal);
    private readonly PriorityQueue<Entry, int> _pending = new();
    private long _version;

    public void Enqueue(string id, int severity)
    {
        Validate(severity);
        if (_current.ContainsKey(id)) throw new InvalidOperationException("Duplicate incident");
        var version = ++_version;
        _current.Add(id, (severity, version));
        _pending.Enqueue(new Entry(id, severity, version), severity);
    }

    public void Revise(string id, int severity)
    {
        Validate(severity);
        if (!_current.ContainsKey(id)) throw new KeyNotFoundException(id);
        var version = ++_version;
        _current[id] = (severity, version);
        _pending.Enqueue(new Entry(id, severity, version), severity);
    }

    public DispatchTicket? DispatchNext()
    {
        while (_pending.TryDequeue(out var entry, out _))
        {
            if (!_current.TryGetValue(entry.Id, out var latest) || latest.Version != entry.Version)
                continue;
            _current.Remove(entry.Id);
            return new DispatchTicket(entry.Id, latest.Severity);
        }
        return null;
    }

    public int PendingCount => _current.Count;

    private static void Validate(int severity)
    {
        if (severity is < 1 or > 9) throw new ArgumentOutOfRangeException(nameof(severity));
    }
}
