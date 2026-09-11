internal sealed class FakeConnectionPool
{
    private readonly SemaphoreSlim _slots;
    private int _inUse;
    private int _waiting;

    public FakeConnectionPool(int capacity)
    {
        Capacity = capacity;
        _slots = new SemaphoreSlim(capacity, capacity);
    }

    public int Capacity { get; }
    public int InUse => Volatile.Read(ref _inUse);
    public int Waiting => Volatile.Read(ref _waiting);

    public async ValueTask<IAsyncDisposable> AcquireAsync(CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _waiting);
        try
        {
            await _slots.WaitAsync(cancellationToken);
        }
        finally
        {
            Interlocked.Decrement(ref _waiting);
        }

        Interlocked.Increment(ref _inUse);
        return new Lease(this);
    }

    private sealed class Lease : IAsyncDisposable
    {
        private readonly FakeConnectionPool _owner;
        private int _released;

        public Lease(FakeConnectionPool owner) => _owner = owner;

        public ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _released, 1) == 0)
            {
                Interlocked.Decrement(ref _owner._inUse);
                _owner._slots.Release();
            }

            return ValueTask.CompletedTask;
        }
    }
}