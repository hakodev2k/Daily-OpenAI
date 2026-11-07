using System.Threading;

namespace ShelterFeedLab;

// Test double cho thao tác warmup có thời gian kết thúc do harness điều khiển.
public sealed class WarmupProbe
{
    private readonly ManualResetEventSlim _release = new(false);

    public TaskCompletionSource<bool> Entered { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public TaskCompletionSource<bool> Completed { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void LoadBlocking(CancellationToken cancellationToken)
    {
        Entered.TrySetResult(true);
        _release.Wait(cancellationToken);
        Completed.TrySetResult(true);
    }

    public void Release() => _release.Set();
}
