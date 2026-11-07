using System.Collections.Generic;

var broker = new InMemoryQueue(new[]
{
    new MessageEnvelope("msg-poison", "template=broken"),
    new MessageEnvelope("msg-good-1", "template=welcome"),
    new MessageEnvelope("msg-good-2", "template=receipt")
});

var handler = new NotificationHandler();
var processed = 0;
const int observationCycles = 6;

for (var cycle = 0; cycle < observationCycles; cycle++)
{
    var message = broker.Receive();
    if (message is null)
    {
        break;
    }

    Console.WriteLine($"RECEIVE id={message.Id} delivery={message.DeliveryCount}");

    try
    {
        handler.Handle(message);
        broker.Complete(message);
        processed++;
        Console.WriteLine($"COMPLETE id={message.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL id={message.Id} type={ex.GetType().Name}");
        broker.Abandon(message);
    }
}

Console.WriteLine("--- SUMMARY ---");
Console.WriteLine($"Processed: {processed}");
Console.WriteLine($"DeadLettered: {broker.DeadLetterCount}");
Console.WriteLine($"Pending: {broker.PendingCount}");

internal sealed class MessageEnvelope
{
    public MessageEnvelope(string id, string payload)
    {
        Id = id;
        Payload = payload;
    }

    public string Id { get; }
    public string Payload { get; }
    public int DeliveryCount { get; set; }
}

internal sealed class InMemoryQueue
{
    private readonly LinkedList<MessageEnvelope> _ready;
    private readonly List<MessageEnvelope> _deadLetters = new();

    public InMemoryQueue(IEnumerable<MessageEnvelope> messages)
    {
        _ready = new LinkedList<MessageEnvelope>(messages);
    }

    public int PendingCount => _ready.Count;
    public int DeadLetterCount => _deadLetters.Count;

    public MessageEnvelope? Receive()
    {
        if (_ready.First is null)
        {
            return null;
        }

        var message = _ready.First.Value;
        _ready.RemoveFirst();
        message.DeliveryCount++;
        return message;
    }

    public void Complete(MessageEnvelope message)
    {
        // The simulator removes a message from the ready queue on Receive.
        // Completion therefore has no additional state transition.
    }

    public void Abandon(MessageEnvelope message)
    {
        _ready.AddFirst(message);
    }

    public void DeadLetter(MessageEnvelope message)
    {
        _deadLetters.Add(message);
    }
}

internal sealed class NotificationHandler
{
    public void Handle(MessageEnvelope message)
    {
        if (message.Id == "msg-poison")
        {
            throw new InvalidDataException("Template payload cannot be rendered.");
        }

        Console.WriteLine($"SEND id={message.Id} payload={message.Payload}");
    }
}
