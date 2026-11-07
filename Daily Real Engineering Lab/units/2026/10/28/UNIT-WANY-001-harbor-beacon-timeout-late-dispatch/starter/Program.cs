using HarborAlertLab;

try
{
    if (args.Length != 1 || (args[0] != "--reproduce" && args[0] != "--verify"))
        throw new ArgumentException("Use --reproduce or --verify.");

    if (args[0] == "--reproduce")
    {
        var transport = new GatedAlertTransport();
        var dispatcher = new AlertDispatcher(transport);
        var first = await dispatcher.SendAsync("BEACON-17", Task.CompletedTask);

        Check(first == DispatchOutcome.TimedOut, "First attempt should report timeout.");
        Check(transport.Active == 1 && transport.Delivered == 0,
            "Starter symptom not reproduced: first send must still be pending.");

        var never = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var retry = dispatcher.SendAsync("BEACON-17", never.Task);
        Check(transport.Active == 2, "Two attempts should be in flight before release.");

        transport.Release();
        Check(await retry == DispatchOutcome.Delivered, "Retry should complete.");
        await transport.WaitForTwoDeliveriesAsync();
        Check(transport.Delivered == 2, "Both attempts should commit after the gate opens.");

        Console.WriteLine("REPRODUCED: timeout returned, original send remained active, retry led to two deliveries.");
    }
    else
    {
        var transport = new GatedAlertTransport();
        var dispatcher = new AlertDispatcher(transport);

        var first = await dispatcher.SendAsync("BEACON-17", Task.CompletedTask);
        Check(first == DispatchOutcome.TimedOut, "Expired attempt must report timeout.");
        Check(transport.Active == 0 && transport.Delivered == 0,
            "Timeout must leave no pending transport work before returning.");

        var never = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var retry = dispatcher.SendAsync("BEACON-17", never.Task);
        Check(transport.Active == 1, "Retry should have exactly one active attempt.");
        transport.Release();
        Check(await retry == DispatchOutcome.Delivered, "Retry should deliver.");
        Check(transport.Delivered == 1 && transport.Active == 0,
            "Exactly one delivery should commit and no attempt should remain active.");

        var cancellationTransport = new GatedAlertTransport();
        var cancellationDispatcher = new AlertDispatcher(cancellationTransport);
        using var callerCts = new CancellationTokenSource();
        var cancelled = cancellationDispatcher.SendAsync("BEACON-18", never.Task, callerCts.Token);
        Check(cancellationTransport.Active == 1, "Caller cancellation case should start one send.");
        callerCts.Cancel();

        var observedCancellation = false;
        try { await cancelled; }
        catch (OperationCanceledException) { observedCancellation = true; }
        Check(observedCancellation, "Caller cancellation must propagate.");
        Check(cancellationTransport.Active == 0 && cancellationTransport.Delivered == 0,
            "Caller cancellation must not leave an active send.");

        var faultDispatcher = new AlertDispatcher(new FaultingAlertTransport());
        var observedFault = false;
        try { await faultDispatcher.SendAsync("BEACON-19", never.Task); }
        catch (InvalidOperationException ex) when (ex.Message == "partner-rejected")
        {
            observedFault = true;
        }
        Check(observedFault, "Transport faults must not be converted to timeout/success.");

        Console.WriteLine("VERIFIED: timeout cleanup, single delivery, caller cancellation, transport fault.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"CHECK FAILED: {ex}");
    Environment.ExitCode = 1;
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

internal sealed class GatedAlertTransport : IAlertTransport
{
    private readonly TaskCompletionSource<bool> _gate =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _twoDelivered =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _active;
    private int _delivered;

    public int Active => Volatile.Read(ref _active);
    public int Delivered => Volatile.Read(ref _delivered);

    public async Task SendAsync(string alertId, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _active);
        try
        {
            await _gate.Task.WaitAsync(cancellationToken);
            if (Interlocked.Increment(ref _delivered) == 2)
                _twoDelivered.TrySetResult(true);
        }
        finally
        {
            Interlocked.Decrement(ref _active);
        }
    }

    public void Release() => _gate.TrySetResult(true);

    public Task WaitForTwoDeliveriesAsync() =>
        _twoDelivered.Task.WaitAsync(TimeSpan.FromSeconds(5));
}

internal sealed class FaultingAlertTransport : IAlertTransport
{
    public Task SendAsync(string alertId, CancellationToken cancellationToken) =>
        Task.FromException(new InvalidOperationException("partner-rejected"));
}
