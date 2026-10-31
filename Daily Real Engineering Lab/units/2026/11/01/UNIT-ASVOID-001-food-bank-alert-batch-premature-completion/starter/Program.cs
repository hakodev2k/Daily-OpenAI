using AlertDispatchLab;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]");
    return 64;
}

var verify = args[0] == "verify";
var alerts = new[]
{
    new Alert("A-101", "north-freezer"),
    new Alert("A-102", "west-freezer"),
    new Alert("A-103", "central-freezer")
};
var gateway = new GatedGateway(alerts.Length);
var dispatcher = new DispatchService();
var batch = dispatcher.DispatchBatchAsync(alerts, gateway, CancellationToken.None);

await gateway.WaitForFirstStartAsync();
bool dispatchTaskCompleted = batch.IsCompleted;
int sentBeforeRelease = gateway.Completed;
Console.WriteLine($"dispatchTaskCompleted={dispatchTaskCompleted}");
Console.WriteLine($"sentBeforeRelease={sentBeforeRelease}");

gateway.Release();
await gateway.WaitForAllCompletedAsync();
await batch.WaitAsync(TimeSpan.FromSeconds(5));
Console.WriteLine($"sentAfterRelease={gateway.Completed}");

if (!verify)
{
    bool symptom = dispatchTaskCompleted && sentBeforeRelease == 0
        && gateway.Completed == alerts.Length;
    Console.WriteLine(symptom ? "REPRODUCED: premature completion" : "NOT REPRODUCED");
    return symptom ? 0 : 2;
}

if (dispatchTaskCompleted || sentBeforeRelease != 0 || gateway.Completed != alerts.Length)
{
    Console.Error.WriteLine("FAIL: batch completion is not tied to all gateway sends");
    return 1;
}

bool failurePropagated = false;
try
{
    await dispatcher.DispatchBatchAsync(
        new[] { new Alert("A-999", "test-freezer") },
        new FailingGateway(),
        CancellationToken.None);
}
catch (InvalidOperationException ex) when (ex.Message == "partner-unavailable")
{
    failurePropagated = true;
}

if (!failurePropagated)
{
    Console.Error.WriteLine("FAIL: gateway failure was not observed by the batch caller");
    return 1;
}

Console.WriteLine("PASS: completion, send count, and failure propagation");
return 0;

internal sealed class GatedGateway : IAlertGateway
{
    private readonly int _expected;
    private readonly TaskCompletionSource _firstStarted =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _released =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _allCompleted =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _started;
    private int _completed;

    public GatedGateway(int expected) => _expected = expected;
    public int Completed => Volatile.Read(ref _completed);

    public async Task SendAsync(Alert alert, CancellationToken cancellationToken)
    {
        if (Interlocked.Increment(ref _started) == 1)
            _firstStarted.TrySetResult();

        await _released.Task.WaitAsync(cancellationToken);

        if (Interlocked.Increment(ref _completed) == _expected)
            _allCompleted.TrySetResult();
    }

    public Task WaitForFirstStartAsync() =>
        _firstStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

    public Task WaitForAllCompletedAsync() =>
        _allCompleted.Task.WaitAsync(TimeSpan.FromSeconds(5));

    public void Release() => _released.TrySetResult();
}

internal sealed class FailingGateway : IAlertGateway
{
    public async Task SendAsync(Alert alert, CancellationToken cancellationToken)
    {
        await Task.Yield();
        throw new InvalidOperationException("partner-unavailable");
    }
}
