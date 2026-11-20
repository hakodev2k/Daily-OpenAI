using System.Threading;

var gate = new FeedingGate();
var max = await gate.ReproduceAsync();
Console.WriteLine($"maxConcurrent={max}");
return max == 1 ? 0 : 2;

sealed class FeedingGate
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private int _inside;
    private int _peak;

    public async Task<int> ReproduceAsync()
    {
        var firstEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = ExecuteAsync(CancellationToken.None, async () =>
        {
            firstEntered.TrySetResult();
            await releaseFirst.Task;
        });
        await firstEntered.Task;

        using var canceled = new CancellationTokenSource();
        canceled.Cancel();
        try { await ExecuteAsync(canceled.Token, () => Task.CompletedTask); }
        catch (OperationCanceledException) { Console.WriteLine("waiting request canceled"); }

        var secondEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseSecond = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var second = ExecuteAsync(CancellationToken.None, async () =>
        {
            secondEntered.TrySetResult();
            await releaseSecond.Task;
        });

        // The first operation must still hold the gate at this checkpoint.
        if (secondEntered.Task.IsCompleted)
            Console.WriteLine("second operation entered before first exited");

        releaseFirst.TrySetResult();
        await first;
        releaseSecond.TrySetResult();
        await second;
        return _peak;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken, Func<Task> action)
    {
        try
        {
            await _gate.WaitAsync(cancellationToken);
            var active = Interlocked.Increment(ref _inside);
            UpdatePeak(active);
            try { await action(); }
            finally { Interlocked.Decrement(ref _inside); }
        }
        finally
        {
            _gate.Release();
        }
    }

    private void UpdatePeak(int active)
    {
        int seen;
        do
        {
            seen = Volatile.Read(ref _peak);
            if (active <= seen) return;
        } while (Interlocked.CompareExchange(ref _peak, active, seen) != seen);
    }
}
