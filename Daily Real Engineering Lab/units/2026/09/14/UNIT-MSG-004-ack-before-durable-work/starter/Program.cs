record Message(string Id, string InvoiceId);

sealed class FakeQueue
{
    private readonly Message _message;
    private bool _completed;

    public FakeQueue(Message message) => _message = message;
    public Message? Receive() => _completed ? null : _message;

    public void Complete(string messageId)
    {
        if (messageId != _message.Id)
            throw new InvalidOperationException("Unknown message.");
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
        if (message is null)
            return;

        queue.Complete(message.Id);

        if (simulateCrash)
            return;

        store.Save(message.InvoiceId);
    }
}

static class Lab
{
    private const string InvoiceId = "INV-2026-0914";

    public static int Reproduce()
    {
        var queue = new FakeQueue(new Message("msg-1", InvoiceId));
        var store = new ProjectionStore();
        var consumer = new InvoiceConsumer(queue, store);

        consumer.ProcessNext(simulateCrash: true);
        consumer.ProcessNext(simulateCrash: false);

        Console.WriteLine($"messageCompleted={queue.IsCompleted}");
        Console.WriteLine($"projectionExists={store.Exists(InvoiceId)}");

        var symptomExists = queue.IsCompleted && !store.Exists(InvoiceId);
        Console.WriteLine(symptomExists ? "SYMPTOM_REPRODUCED" : "SYMPTOM_NOT_REPRODUCED");
        return symptomExists ? 0 : 1;
    }

    public static int Verify()
    {
        var happyQueue = new FakeQueue(new Message("happy", InvoiceId));
        var happyStore = new ProjectionStore();
        var happyConsumer = new InvoiceConsumer(happyQueue, happyStore);
        happyConsumer.ProcessNext(simulateCrash: false);
        var happyPathOk = happyQueue.IsCompleted && happyStore.Exists(InvoiceId);

        var recoveryQueue = new FakeQueue(new Message("recovery", InvoiceId));
        var recoveryStore = new ProjectionStore();
        var recoveryConsumer = new InvoiceConsumer(recoveryQueue, recoveryStore);
        recoveryConsumer.ProcessNext(simulateCrash: true);
        recoveryConsumer.ProcessNext(simulateCrash: false);
        var recoveryOk = recoveryQueue.IsCompleted && recoveryStore.Exists(InvoiceId);

        Console.WriteLine($"happyPath={happyPathOk}");
        Console.WriteLine($"crashRecovery={recoveryOk}");
        return happyPathOk && recoveryOk ? 0 : 1;
    }
}

static class Program
{
    public static int Main(string[] args)
    {
        var mode = args.FirstOrDefault() ?? "--reproduce";
        return mode switch
        {
            "--reproduce" => Lab.Reproduce(),
            "--verify" => Lab.Verify(),
            _ => throw new ArgumentException($"Unknown mode: {mode}")
        };
    }
}
