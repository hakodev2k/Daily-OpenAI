using WaterReports;

try
{
    var first = new ControlledGateway(shouldReject: false);
    Task pending = new ReportDispatcher(first).DispatchAsync();
    await first.Started.WaitAsync(TimeSpan.FromSeconds(5));
    bool completedWhileGatewayBlocked = await Task.WhenAny(pending, Task.Delay(300)) == pending;
    first.Release();
    await pending.WaitAsync(TimeSpan.FromSeconds(5));
    bool waitPassed = !completedWhileGatewayBlocked;
    Console.WriteLine($"CASE=WAITS_FOR_SEND RESULT={(waitPassed ? "PASS" : "FAIL")}");

    var second = new ControlledGateway(shouldReject: true);
    Task rejected = new ReportDispatcher(second).DispatchAsync();
    await second.Started.WaitAsync(TimeSpan.FromSeconds(5));
    second.Release();
    bool propagated = false;
    try
    {
        await rejected.WaitAsync(TimeSpan.FromSeconds(5));
    }
    catch (InvalidOperationException ex) when (ex.Message == "PARTNER_REJECTED")
    {
        propagated = true;
    }
    Console.WriteLine($"CASE=PROPAGATES_FAILURE RESULT={(propagated ? "PASS" : "FAIL")}");

    var third = new ControlledGateway(shouldReject: false);
    Task accepted = new ReportDispatcher(third).DispatchAsync();
    await third.Started.WaitAsync(TimeSpan.FromSeconds(5));
    third.Release();
    await accepted.WaitAsync(TimeSpan.FromSeconds(5));
    bool successPassed = third.StartedCount == 1 && third.CompletedCount == 1;
    Console.WriteLine($"CASE=SUCCESS_PATH RESULT={(successPassed ? "PASS" : "FAIL")}");

    bool allPassed = waitPassed && propagated && successPassed;
    Console.WriteLine($"LAB_RESULT={(allPassed ? "PASS" : "FAIL")}");
    Environment.ExitCode = allPassed ? 0 : 2;
}
catch (Exception ex)
{
    Console.WriteLine($"HARNESS_ERROR={ex.GetType().Name}:{ex.Message}");
    Console.WriteLine("LAB_RESULT=ERROR");
    Environment.ExitCode = 3;
}

internal sealed class ControlledGateway : IReportGateway
{
    private readonly TaskCompletionSource<bool> _release =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _started =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly bool _shouldReject;
    private int _startedCount;
    private int _completedCount;

    public ControlledGateway(bool shouldReject) => _shouldReject = shouldReject;

    public Task Started => _started.Task;
    public int StartedCount => Volatile.Read(ref _startedCount);
    public int CompletedCount => Volatile.Read(ref _completedCount);

    public void Release() => _release.TrySetResult(true);

    public async Task SendAsync(CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _startedCount);
        _started.TrySetResult(true);
        await _release.Task.WaitAsync(cancellationToken);
        if (_shouldReject)
            throw new InvalidOperationException("PARTNER_REJECTED");
        Interlocked.Increment(ref _completedCount);
    }
}
