var messages = new[]
{
    new Message("m1", "order-101", true),
    new Message("m2", "order-102", true),
    new Message("m3", "order-bad", false),
    new Message("m4", "order-104", true)
};

var broker = new Broker(messages);
var effects = new Dictionary<string, int>();

for (var attempt = 1; attempt <= 2; attempt++)
{
    var batch = broker.ReceiveBatch();
    Console.WriteLine($"ATTEMPT {attempt}: {string.Join(',', batch.Select(x => x.Id))}");
    try
    {
        foreach (var message in batch)
        {
            if (!message.IsValid)
                throw new InvalidOperationException($"Malformed {message.Id}");

            effects[message.OrderId] = effects.GetValueOrDefault(message.OrderId) + 1;
            Console.WriteLine($"SIDE_EFFECT {message.OrderId} count={effects[message.OrderId]}");
        }

        broker.CompleteBatch(batch);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"BATCH_FAILED {ex.Message}");
    }
}

foreach (var pair in effects.OrderBy(x => x.Key))
    Console.WriteLine($"FINAL {pair.Key}={pair.Value}");

record Message(string Id, string OrderId, bool IsValid);

sealed class Broker
{
    private readonly List<Message> _messages;
    private readonly HashSet<string> _completed = new();

    public Broker(IEnumerable<Message> messages) => _messages = messages.ToList();

    public IReadOnlyList<Message> ReceiveBatch() => _messages.Where(x => !_completed.Contains(x.Id)).ToList();

    public void CompleteBatch(IEnumerable<Message> batch)
    {
        foreach (var message in batch)
            _completed.Add(message.Id);
    }
}