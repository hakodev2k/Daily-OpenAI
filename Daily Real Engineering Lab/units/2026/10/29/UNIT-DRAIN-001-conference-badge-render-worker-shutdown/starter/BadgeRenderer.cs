namespace BadgeDrainLab;

internal sealed class BadgeRenderer
{
    private readonly JobStore _store;
    private readonly Task _releaseGate;
    private readonly bool _failAfterPublish;
    private readonly TaskCompletionSource<bool>? _started;

    public BadgeRenderer(
        JobStore store, Task releaseGate, bool failAfterPublish = false,
        TaskCompletionSource<bool>? started = null)
    {
        _store = store;
        _releaseGate = releaseGate;
        _failAfterPublish = failAfterPublish;
        _started = started;
    }

    public async Task RenderAsync(string jobId, CancellationToken cancellationToken)
    {
        _started?.TrySetResult(true);
        await _releaseGate.WaitAsync(cancellationToken);
        _store.Publish(jobId);
        if (_failAfterPublish)
            throw new IOException("Simulated receipt failure after artifact storage.");
    }
}
