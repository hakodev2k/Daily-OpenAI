using Lab;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- <reproduce|verify>");
    return 2;
}

var mode = args[0];
var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
var active = 0;
var maxActive = 0;
var started = 0;
var completed = 0;

async Task ProcessBatch(CancellationToken cancellationToken)
{
    var current = Interlocked.Increment(ref active);
    while (true)
    {
        var previous = Volatile.Read(ref maxActive);
        if (previous >= current || Interlocked.CompareExchange(ref maxActive, current, previous) == previous)
            break;
    }

    var batchId = Interlocked.Increment(ref started);
    Console.WriteLine($"BATCH_START id={batchId} active={current}");
    firstStarted.TrySetResult(true);
    try
    {
        await gate.Task.WaitAsync(cancellationToken);
        Interlocked.Increment(ref completed);
        Console.WriteLine($"BATCH_COMPLETE id={batchId}");
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
        Console.WriteLine($"BATCH_CANCELLED id={batchId}");
    }
    finally
    {
        Interlocked.Decrement(ref active);
    }
}

var runner = new JobRunner(ProcessBatch, TimeSpan.FromMilliseconds(20));
runner.Start();
var startedAtStop = -1;
try
{
    await firstStarted.Task.WaitAsync(TimeSpan.FromSeconds(3));
    await Task.Delay(180);
    await runner.StopAsync();
    startedAtStop = Volatile.Read(ref started);
}
finally
{
    gate.TrySetResult(true);
}

await Task.Delay(120);
var observedMax = Volatile.Read(ref maxActive);
var finalActive = Volatile.Read(ref active);
var finalStarted = Volatile.Read(ref started);
Console.WriteLine($"EVIDENCE mode={mode} started={finalStarted} completed={Volatile.Read(ref completed)} maxActive={observedMax} activeAfterStop={finalActive} startedAfterStop={finalStarted - startedAtStop}");

if (mode == "reproduce")
{
    var reproduced = observedMax >= 2 && finalActive == 0;
    Console.WriteLine($"REPRODUCED={reproduced}");
    return reproduced ? 0 : 1;
}

var verified = observedMax == 1 && finalStarted >= 1 && finalActive == 0 && finalStarted == startedAtStop;
Console.WriteLine($"VERIFIED={verified}");
return verified ? 0 : 1;
