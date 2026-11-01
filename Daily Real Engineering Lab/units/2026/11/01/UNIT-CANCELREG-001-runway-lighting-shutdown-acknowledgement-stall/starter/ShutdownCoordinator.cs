namespace RunwayLighting;
public sealed class ShutdownCoordinator : IDisposable
{
    private readonly CancellationTokenRegistration _registration;
    private readonly TaskCompletionSource<bool> _entered = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _finished = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _flushCount;
    public Task CallbackEntered => _entered.Task;
    public Task CleanupCompleted => _finished.Task;
    public int FlushCount => Volatile.Read(ref _flushCount);
    public ShutdownCoordinator(CancellationToken token, Task downstreamRelease)
    {
        _registration = token.Register(() =>
        {
            _entered.TrySetResult(true);
            downstreamRelease.GetAwaiter().GetResult();
            Interlocked.Increment(ref _flushCount);
            _finished.TrySetResult(true);
        });
    }
    public void Dispose() => _registration.Dispose();
}
