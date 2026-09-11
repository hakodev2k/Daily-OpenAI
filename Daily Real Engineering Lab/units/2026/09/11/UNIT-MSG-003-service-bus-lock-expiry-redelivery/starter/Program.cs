using System.Collections.Concurrent;

var broker = new LeaseBroker(TimeSpan.FromMilliseconds(200));
var worker = new PreviewWorker();
var result = await broker.RunAsync("doc-8472", worker.ProcessAsync);

Console.WriteLine($"DELIVERIES={result.Deliveries}");
Console.WriteLine($"MAX_CONCURRENT_FOR_MESSAGE={result.MaxConcurrent}");
Console.WriteLine($"SIDE_EFFECTS={result.SideEffects}");
Console.WriteLine($"COMPLETIONS={result.AcceptedCompletions}");

sealed class PreviewWorker
{
    public async Task ProcessAsync(Delivery delivery)
    {
        Console.WriteLine($"START message={delivery.MessageId} delivery={delivery.DeliveryNumber}");

        // A large document takes longer than most messages.
        await Task.Delay(500);

        delivery.RecordSideEffect();
        Console.WriteLine($"OUTPUT_WRITTEN message={delivery.MessageId} delivery={delivery.DeliveryNumber}");
        delivery.Complete();
        Console.WriteLine($"COMPLETE_ATTEMPT message={delivery.MessageId} delivery={delivery.DeliveryNumber}");
    }
}

sealed class LeaseBroker
{
    private readonly TimeSpan _lockDuration;

    public LeaseBroker(TimeSpan lockDuration) => _lockDuration = lockDuration;

    public async Task<BrokerResult> RunAsync(string messageId, Func<Delivery, Task> handler)
    {
        var state = new MessageState(messageId, _lockDuration);
        var tasks = new List<Task>();
        StartDelivery(state, handler, tasks);

        while (!state.IsCompleted)
        {
            await Task.Delay(50);
            if (!state.IsCompleted && state.IsLockExpired())
            {
                Console.WriteLine($"LOCK_EXPIRED message={messageId}");
                StartDelivery(state, handler, tasks);
            }
        }

        await Task.WhenAll(tasks.ToArray());
        return state.ToResult();
    }

    private void StartDelivery(MessageState state, Func<Delivery, Task> handler, List<Task> tasks)
    {
        var delivery = state.BeginDelivery();
        tasks.Add(RunHandlerAsync(state, delivery, handler));
    }

    private static async Task RunHandlerAsync(MessageState state, Delivery delivery, Func<Delivery, Task> handler)
    {
        state.HandlerStarted();
        try
        {
            await handler(delivery);
        }
        finally
        {
            state.HandlerFinished();
        }
    }
}

sealed class Delivery
{
    private readonly MessageState _state;

    internal Delivery(MessageState state, int deliveryNumber)
    {
        _state = state;
        DeliveryNumber = deliveryNumber;
    }

    public string MessageId => _state.MessageId;
    public int DeliveryNumber { get; }

    public void RenewLock()
    {
        _state.RenewLock();
        Console.WriteLine($"LOCK_RENEWED message={MessageId} delivery={DeliveryNumber}");
    }

    public void RecordSideEffect() => _state.RecordSideEffect();
    public void Complete() => _state.Complete();
}

sealed class MessageState
{
    private readonly object _gate = new();
    private readonly TimeSpan _lockDuration;
    private DateTimeOffset _lockedUntil;
    private int _deliveries;
    private int _activeHandlers;
    private int _maxConcurrent;
    private int _sideEffects;
    private int _acceptedCompletions;
    private bool _completed;

    public MessageState(string messageId, TimeSpan lockDuration)
    {
        MessageId = messageId;
        _lockDuration = lockDuration;
    }

    public string MessageId { get; }

    public bool IsCompleted
    {
        get { lock (_gate) return _completed; }
    }

    public Delivery BeginDelivery()
    {
        lock (_gate)
        {
            _deliveries++;
            _lockedUntil = DateTimeOffset.UtcNow + _lockDuration;
            return new Delivery(this, _deliveries);
        }
    }

    public bool IsLockExpired()
    {
        lock (_gate) return DateTimeOffset.UtcNow >= _lockedUntil;
    }

    public void RenewLock()
    {
        lock (_gate)
        {
            if (!_completed)
                _lockedUntil = DateTimeOffset.UtcNow + _lockDuration;
        }
    }

    public void HandlerStarted()
    {
        lock (_gate)
        {
            _activeHandlers++;
            _maxConcurrent = Math.Max(_maxConcurrent, _activeHandlers);
        }
    }

    public void HandlerFinished()
    {
        lock (_gate) _activeHandlers--;
    }

    public void RecordSideEffect()
    {
        lock (_gate) _sideEffects++;
    }

    public void Complete()
    {
        lock (_gate)
        {
            if (_completed) return;
            _completed = true;
            _acceptedCompletions++;
        }
    }

    public BrokerResult ToResult()
    {
        lock (_gate)
            return new BrokerResult(_deliveries, _maxConcurrent, _sideEffects, _acceptedCompletions);
    }
}

readonly record struct BrokerResult(int Deliveries, int MaxConcurrent, int SideEffects, int AcceptedCompletions);
