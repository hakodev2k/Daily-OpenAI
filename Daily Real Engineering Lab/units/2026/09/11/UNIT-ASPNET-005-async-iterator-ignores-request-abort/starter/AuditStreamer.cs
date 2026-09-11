internal sealed class AuditStreamer
{
    private readonly FakeConnectionPool _pool;
    private int _activeProducers;

    public AuditStreamer(FakeConnectionPool pool) => _pool = pool;

    public int ActiveProducers => Volatile.Read(ref _activeProducers);

    // Investigation note:
    // The consumer supplies cancellation while enumerating.
    // Verify which token the iterator body actually observes.
    public async IAsyncEnumerable<string> StreamAsync(CancellationToken cancellationToken = default)
    {
        await using var lease = await _pool.AcquireAsync(cancellationToken);
        Interlocked.Increment(ref _activeProducers);

        try
        {
            for (var i = 0; i < 30; i++)
            {
                await Task.Delay(100, cancellationToken);
                yield return $"event-{i}";
            }
        }
        finally
        {
            Interlocked.Decrement(ref _activeProducers);
        }
    }
}