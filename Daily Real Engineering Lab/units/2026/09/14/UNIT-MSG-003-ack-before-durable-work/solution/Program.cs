record Message(string Id, string InvoiceId);

sealed class FakeQueue
{
    private readonly Message _message;
    private bool _completed;

    public FakeQueue(Message message) => _message = message;
    public Message? Receive() => _completed ? null : _message;
    public void Complete(string messageId)
    {
        if (messageId != _message.Id) throw new InvalidOperationException("Unknown message.");
        _completed = true;
    }
    public bool IsCompleted => _completed;
}

sealed class ProjectionStore
{
    private readonly HashSet<string> _invoiceIds = new();
    public void Save(string invoiceId) => _invoiceIds.Add(invoiceId);
    public bool Exists(string invoiceId) => _invoiceIds.Contains(invoiceId);
}

sealed class InvoiceConsumer(FakeQueue queue, ProjectionStore store)
{
    public void ProcessNext(bool simulateCrash)
    {
        var message = queue.Receive();
        if (message is null) return;

        if (simulateCrash) return;

        store.Save(message.InvoiceId);
        queue.Complete(message.Id);
    }
}

const string invoiceId = "INV-2026-0914";
var queue = new FakeQueue(new Message("msg-1", invoiceId));
var store = new ProjectionStore();
var consumer = new InvoiceConsumer(queue, store);

consumer.ProcessNext(simulateCrash: true);
consumer.ProcessNext(simulateCrash: false);

var ok = queue.IsCompleted && store.Exists(invoiceId);
Console.WriteLine($"messageCompleted={queue.IsCompleted}");
Console.WriteLine($"projectionExists={store.Exists(invoiceId)}");
Console.WriteLine(ok ? "REFERENCE_PASS" : "REFERENCE_FAIL");
return ok ? 0 : 1;
