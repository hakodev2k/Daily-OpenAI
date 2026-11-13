using System.Threading;
var gate = new SemaphoreSlim(1, 2);
var aEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var allowAExit = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
int active = 0, peak = 0;
async Task Process(string name, CancellationToken cancellation, Task? hold = null)
{
    Console.WriteLine($"{name}: waiting");
    try
    {
        await gate.WaitAsync(cancellation);
        int now = Interlocked.Increment(ref active);
        int old;
        do { old = Volatile.Read(ref peak); if (now <= old) break; }
        while (Interlocked.CompareExchange(ref peak, now, old) != old);
        Console.WriteLine($"{name}: entered");
        if (name == "A") aEntered.SetResult();
        try { if (hold is not null) await hold; else await Task.Delay(15); }
        finally { Interlocked.Decrement(ref active); }
    }
    catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
    {
        Console.WriteLine($"{name}: cancelled");
    }
    finally { gate.Release(); }
}
var a = Process("A", CancellationToken.None, allowAExit.Task);
await aEntered.Task;
using var cts = new CancellationTokenSource();
cts.Cancel();
await Process("B", cts.Token);
var c = Process("C", CancellationToken.None);
await Task.Delay(60);
allowAExit.SetResult();
await Task.WhenAll(a, c);
Console.WriteLine($"MAX_ACTIVE={peak}");
