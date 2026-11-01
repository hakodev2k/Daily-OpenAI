namespace RunwayLighting;
public sealed class ShutdownCoordinator : IDisposable
{
    private readonly CancellationTokenRegistration _registration;
    private readonly TaskCompletionSource<bool> _entered = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _stopSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Task _cleanupTask;
    private int _flushCount;
    public Task CallbackEntered => _entered.Task;
    public Task CleanupCompleted => _cleanupTask;
    public int FlushCount => Volatile.Read(ref _flushCount);
    public ShutdownCoordinator(CancellationToken token, Task downstreamRelease)
    {
        _cleanupTask = FlushAfterSignalAsync(downstreamRelease);
        _registration = token.Register(() =>
        {
            _entered.TrySetResult(true);
            _stopSignal.TrySetResult(true);
        });
    }
    private async Task FlushAfterSignalAsync(Task downstreamRelease)
    {
        await _stopSignal.Task.ConfigureAwait(false);
        await downstreamRelease.ConfigureAwait(false);
        Interlocked.Increment(ref _flushCount);
    }
    public void Dispose() => _registration.Dispose();
}
