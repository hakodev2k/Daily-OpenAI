using IrrigationGate;

var mode = args.FirstOrDefault() ?? "verify";
if (mode is not ("reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- reproduce|verify");
    return 2;
}

static async Task<(bool canceled, bool firstCompleted, bool overlap, bool releaseFault, int executed)> ProbeAsync()
{
    var controller = new IrrigationController();
    var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var unblock = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var first = controller.ExecuteAsync(async () =>
    {
        entered.SetResult();
        await unblock.Task;
    }, CancellationToken.None);
    await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));

    using var cts = new CancellationTokenSource();
    cts.Cancel();
    bool canceled = false, overlap = false, releaseFault = false;
    int executed = 0;
    try
    {
        await controller.ExecuteAsync(() =>
        {
            Interlocked.Increment(ref executed);
            return Task.CompletedTask;
        }, cts.Token);
    }
    catch (OperationCanceledException) { canceled = true; }
    catch (SemaphoreFullException) { releaseFault = true; }

    // A third caller must not enter while the first one still owns the permit.
    var thirdEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    using var thirdCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    var third = controller.ExecuteAsync(() =>
    {
        thirdEntered.TrySetResult();
        return Task.CompletedTask;
    }, thirdCts.Token);
    await Task.WhenAny(thirdEntered.Task, Task.Delay(100));
    overlap = thirdEntered.Task.IsCompleted;

    unblock.SetResult();
    bool firstCompleted = false;
    try { await first.WaitAsync(TimeSpan.FromSeconds(5)); firstCompleted = true; }
    catch (SemaphoreFullException) { releaseFault = true; }
    try { await third.WaitAsync(TimeSpan.FromSeconds(5)); }
    catch (SemaphoreFullException) { releaseFault = true; }
    catch (OperationCanceledException) { }
    return (canceled, firstCompleted, overlap, releaseFault, executed);
}

var p = await ProbeAsync();
Console.WriteLine($"canceled={p.canceled} firstCompleted={p.firstCompleted} overlap={p.overlap} releaseFault={p.releaseFault} executed={p.executed}");
if (mode == "reproduce")
{
    bool observed = p.overlap || p.releaseFault || !p.canceled || p.executed != 0;
    Console.WriteLine(observed ? "REPRODUCED: gate ownership contract violated" : "NOT REPRODUCED: contract already satisfied");
    return observed ? 0 : 1;
}
bool valid = p.canceled && p.firstCompleted && !p.overlap && !p.releaseFault && p.executed == 0;
Console.WriteLine(valid ? "PASS: cancellation and exclusivity contracts hold" : "FAIL: cancellation or exclusivity contract broken");
return valid ? 0 : 1;
