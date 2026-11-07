using PermitGateLab;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
if (mode is not ("demo" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: demo | reproduce | verify");
    return 2;
}

try
{
    var (enteredEarly, cancellationObserved, faultRecovery) = await ProbeAsync();
    Console.WriteLine($"A still running when C entered: {enteredEarly}");
    Console.WriteLine($"B cancellation observed: {cancellationObserved}");
    Console.WriteLine($"Renderer exception did not poison gate: {faultRecovery}");

    var guardrailsPass = cancellationObserved && faultRecovery;
    if (mode == "demo") return guardrailsPass ? 0 : 1;
    if (mode == "reproduce")
    {
        Console.WriteLine(enteredEarly && guardrailsPass ? "REPRODUCED" : "NOT REPRODUCED");
        return enteredEarly && guardrailsPass ? 0 : 1;
    }

    Console.WriteLine(!enteredEarly && guardrailsPass ? "VERIFIED" : "CONTRACT FAILED");
    return !enteredEarly && guardrailsPass ? 0 : 1;
}
catch (Exception error)
{
    Console.Error.WriteLine($"Harness error: {error.GetType().Name}: {error.Message}");
    return 2;
}

static async Task<(bool EnteredEarly, bool CancellationObserved, bool FaultRecovery)> ProbeAsync()
{
    var gate = new WorkGate(1);
    var firstEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var thirdEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

    var first = gate.RunAsync(async token =>
    {
        firstEntered.TrySetResult();
        await releaseFirst.Task.WaitAsync(token);
    }, CancellationToken.None);

    await firstEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
    using var canceledRequest = new CancellationTokenSource();

    var second = gate.RunAsync(_ => Task.CompletedTask, canceledRequest.Token);
    if (second.IsCompleted)
        throw new InvalidOperationException("Expected B to be queued while A holds capacity.");

    canceledRequest.Cancel();
    var cancellationObserved = false;
    try
    {
        await second;
    }
    catch (OperationCanceledException)
    {
        cancellationObserved = true;
    }

    var third = gate.RunAsync(_ =>
    {
        thirdEntered.TrySetResult();
        return Task.CompletedTask;
    }, CancellationToken.None);

    // The callback starts synchronously if capacity is incorrectly available.
    var enteredEarly = thirdEntered.Task.IsCompleted;
    releaseFirst.TrySetResult();

    await Task.WhenAll(first, third).WaitAsync(TimeSpan.FromSeconds(5));

    var recoveryGate = new WorkGate(1);
    var exceptionObserved = false;
    try
    {
        await recoveryGate.RunAsync(
            _ => Task.FromException(new InvalidOperationException("simulated render failure")),
            CancellationToken.None);
    }
    catch (InvalidOperationException)
    {
        exceptionObserved = true;
    }

    var subsequentRan = false;
    await recoveryGate.RunAsync(_ =>
    {
        subsequentRan = true;
        return Task.CompletedTask;
    }, CancellationToken.None);

    return (enteredEarly, cancellationObserved, exceptionObserved && subsequentRan);
}
