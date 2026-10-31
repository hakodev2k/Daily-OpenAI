namespace PatrolLab;

public sealed record PatrolNotice(string PatrolId, string RecipientId, string Text);

public sealed class DeferredQueue
{
    private readonly List<Action> _pending = new();

    public int PendingCount => _pending.Count;

    public void Enqueue(Action action) => _pending.Add(action);

    public void Drain()
    {
        foreach (var action in _pending)
        {
            action();
        }

        _pending.Clear();
    }
}

public sealed class FakeGateway
{
    private readonly List<PatrolNotice> _sent = new();

    public IReadOnlyList<PatrolNotice> Sent => _sent;

    public void Send(PatrolNotice notice) => _sent.Add(notice);
}
