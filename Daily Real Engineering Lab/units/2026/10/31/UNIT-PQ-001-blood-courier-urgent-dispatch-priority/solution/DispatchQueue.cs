namespace DispatchLab;

/// <summary>
/// Một dispatcher thread. Các ticket cũ có thể tồn tại trong heap
/// nhưng không bao giờ được điều phối lần thứ hai.
/// </summary>
public sealed class DispatchQueue
{
    private readonly record struct Ticket(string Id, long Version);

    private sealed class Request
    {
        public required string Id { get; init; }
        public int Urgency { get; set; }
        public long Version { get; set; }
        public long Sequence { get; init; }
    }

    private readonly Dictionary<string, Request> _pending = new(StringComparer.Ordinal);
    private readonly PriorityQueue<Ticket, (int Urgency, long Sequence)> _queue = new();
    private long _sequence;

    public int PendingCount => _pending.Count;

    public void Register(string id, int urgency)
    {
        Validate(id, urgency);
        if (_pending.ContainsKey(id))
            throw new ArgumentException("Request already pending.", nameof(id));

        var request = new Request
        {
            Id = id,
            Urgency = urgency,
            Version = 1,
            Sequence = _sequence++
        };
        _pending.Add(id, request);
        EnqueueCurrent(request);
    }

    public bool Escalate(string id, int newUrgency)
    {
        Validate(id, newUrgency);
        if (!_pending.TryGetValue(id, out var request))
            return false;
        if (newUrgency >= request.Urgency)
            return false;

        request.Urgency = newUrgency;
        request.Version++;
        EnqueueCurrent(request);
        return true;
    }

    public string? DispatchNext()
    {
        while (_queue.TryDequeue(out var ticket, out _))
        {
            if (!_pending.TryGetValue(ticket.Id, out var request))
                continue;
            if (request.Version != ticket.Version)
                continue;

            _pending.Remove(ticket.Id);
            return ticket.Id;
        }

        return null;
    }

    private void EnqueueCurrent(Request request) =>
        _queue.Enqueue(new Ticket(request.Id, request.Version),
            (request.Urgency, request.Sequence));

    private static void Validate(string id, int urgency)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Request ID is required.", nameof(id));
        if (urgency is < 0 or > 9)
            throw new ArgumentOutOfRangeException(nameof(urgency));
    }
}
