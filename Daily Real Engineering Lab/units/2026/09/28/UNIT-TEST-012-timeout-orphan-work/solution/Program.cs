using System.Collections.Concurrent;

var fixture = new ConcurrentQueue<string>();

Console.WriteLine("TEST-1 start");
try
{
    await RunWithTimeoutAsync(
        token => SimulatedDependencyAsync("late-write", fixture, token),
        TimeSpan.FromMilliseconds(80));
}
catch (TimeoutException)
{
    Console.WriteLine("TEST-1 timeout observed");
}

Console.WriteLine("TEST-2 start");
await Task.Delay(180);

if (fixture.TryPeek(out var leaked))
{
    Console.WriteLine($"ORPHANED_WORK_DETECTED: {leaked}");
    Environment.ExitCode = 1;
}
else
{
    Console.WriteLine("NO_ORPHANED_WORK");
}

static async Task RunWithTimeoutAsync(
    Func<CancellationToken, Task> operation,
    TimeSpan timeout)
{
    using var timeoutCts = new CancellationTokenSource(timeout);

    try
    {
        await operation(timeoutCts.Token);
    }
    catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
    {
        throw new TimeoutException("Operation exceeded test timeout.");
    }
}

static async Task SimulatedDependencyAsync(
    string value,
    ConcurrentQueue<string> fixture,
    CancellationToken cancellationToken)
{
    await Task.Delay(180, cancellationToken);
    fixture.Enqueue(value);
}
