using System.Collections.Concurrent;

var broker = new FakeBroker(TimeSpan.FromMilliseconds(120));
var effects = new ConcurrentDictionary<string, int>();
await broker.PublishAsync(new WorkItem("doc-42", TimeSpan.FromMilliseconds(220)));

async Task HandleAsync(Delivery d)
{
    Console.WriteLine($"START id={d.Item.Id} delivery={d.DeliveryCount} token={d.Token}");
    await Task.Delay(d.Item.Duration);
    effects.AddOrUpdate(d.Item.Id, 1, (_, n) => n + 1);
    Console.WriteLine($"SIDE_EFFECT id={d.Item.Id} count={effects[d.Item.Id]}");
    await broker.CompleteAsync(d);
}

await broker.RunAsync(HandleAsync, TimeSpan.FromMilliseconds(520));
var count = effects.GetValueOrDefault("doc-42");
Console.WriteLine($"FINAL_SIDE_EFFECTS={count}");
return count == 1 ? 0 : 2;

record WorkItem(string Id, TimeSpan Duration);
record Delivery(WorkItem Item, int DeliveryCount, Guid Token, DateTimeOffset LockedUntil);

sealed class FakeBroker
{
    private readonly TimeSpan _lockDuration;
    private readonly Queue<WorkItem> _pending = new();
    private readonly Dictionary<string, int> _counts = new();
    private readonly ConcurrentDictionary<Guid, Delivery> _active = new();
    public FakeBroker(TimeSpan lockDuration) => _lockDuration = lockDuration;
    public Task PublishAsync(WorkItem item) { _pending.Enqueue(item); return Task.CompletedTask; }

    public async Task RunAsync(Func<Delivery, Task> handler, TimeSpan window)
    {
        var stop = DateTimeOffset.UtcNow + window;
        var running = new List<Task>();
        while (DateTimeOffset.UtcNow < stop)
        {
            foreach (var expired in _active.Values.Where(x => x.LockedUntil <= DateTimeOffset.UtcNow).ToArray())
                if (_active.TryRemove(expired.Token, out _)) _pending.Enqueue(expired.Item);

            if (_pending.TryDequeue(out var item))
            {
                var count = _counts.TryGetValue(item.Id, out var n) ? n + 1 : 1;
                _counts[item.Id] = count;
                var d = new Delivery(item, count, Guid.NewGuid(), DateTimeOffset.UtcNow + _lockDuration);
                _active[d.Token] = d;
                running.Add(Task.Run(() => handler(d)));
            }
            await Task.Delay(15);
        }
        await Task.WhenAll(running);
    }

    public Task CompleteAsync(Delivery delivery)
    {
        if (!_active.TryRemove(delivery.Token, out _))
            throw new InvalidOperationException($"Message lock lost for delivery {delivery.DeliveryCount}");
        return Task.CompletedTask;
    }

    public Task RenewAsync(Delivery delivery)
    {
        if (!_active.TryGetValue(delivery.Token, out var current)) throw new InvalidOperationException("Cannot renew lost lock");
        _active[delivery.Token] = current with { LockedUntil = DateTimeOffset.UtcNow + _lockDuration };
        return Task.CompletedTask;
    }
}