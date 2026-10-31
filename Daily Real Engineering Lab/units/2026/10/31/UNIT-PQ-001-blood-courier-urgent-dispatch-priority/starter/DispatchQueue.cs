namespace DispatchLab;

/// <summary>
/// Bộ điều phối dùng trên một dispatcher thread. Không hỗ trợ gọi đồng thời.
/// 0 = khẩn cấp nhất, 9 = ít khẩn nhất.
/// </summary>
public sealed class DispatchQueue
{
    private sealed class Request
    {
        public required string Id { get; init; }
        public int Urgency { get; set; }
    }

    private readonly Dictionary<string, Request> _pending = new(StringComparer.Ordinal);
    private readonly PriorityQueue<Request, (int Urgency, long Sequence)> _queue = new();
    private long _sequence;

    public int PendingCount => _pending.Count;

    public void Register(string id, int urgency)
    {
        Validate(id, urgency);
        if (_pending.ContainsKey(id))
            throw new ArgumentException("Request already pending.", nameof(id));

        var request = new Request { Id = id, Urgency = urgency };
        _pending.Add(id, request);
        _queue.Enqueue(request, (urgency, _sequence++));
    }

    public bool Escalate(string id, int newUrgency)
    {
        Validate(id, newUrgency);
        if (!_pending.TryGetValue(id, out var request))
            return false;
        if (newUrgency >= request.Urgency)
            return false;

        request.Urgency = newUrgency;
        return true;
    }

    public string? DispatchNext()
    {
        if (!_queue.TryDequeue(out var request, out _))
            return null;

        _pending.Remove(request.Id);
        return request.Id;
    }

    private static void Validate(string id, int urgency)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Request ID is required.", nameof(id));
        if (urgency is < 0 or > 9)
            throw new ArgumentOutOfRangeException(nameof(urgency));
    }
}
