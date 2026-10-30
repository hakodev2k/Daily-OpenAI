namespace DeicingReadiness;
public sealed class HeartbeatCoordinator : IAsyncDisposable
{
    private readonly PeriodicTimer _clock;
    private long _sequence;
    public HeartbeatCoordinator(TimeSpan interval) => _clock = new PeriodicTimer(interval);
    public async Task<long[]> ObserveAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        var samples = new List<long>(count);
        while (samples.Count < count && await _clock.WaitForNextTickAsync(cancellationToken))
            samples.Add(Interlocked.Increment(ref _sequence));
        return samples.ToArray();
    }
    public ValueTask DisposeAsync() { _clock.Dispose(); return ValueTask.CompletedTask; }
}
