using Lab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "demo";
        if (mode is not ("demo" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: demo | reproduce | verify");
            return 2;
        }

        try
        {
            var observed = await ObserveAsync();
            Console.WriteLine($"FirstOutcome={observed.FirstOutcome?.ToString() ?? "none"}");
            Console.WriteLine($"FirstError={observed.FirstError?.GetType().Name ?? "none"}");
            Console.WriteLine($"SecondOutcome={observed.SecondOutcome?.ToString() ?? "none"}");
            Console.WriteLine($"SecondError={observed.SecondError?.GetType().Name ?? "none"}");
            Console.WriteLine($"SecondWorkRan={observed.SecondWorkRan}");
            Console.WriteLine($"MaxConcurrentWork={observed.MaxConcurrentWork}");
            Console.WriteLine($"ThirdOutcome={observed.ThirdOutcome?.ToString() ?? "none"}");
            Console.WriteLine($"WorkFailurePropagated={observed.WorkFailurePropagated}");
            Console.WriteLine($"PermitRestoredAfterFailure={observed.PermitRestoredAfterFailure}");
            Console.WriteLine($"CancellationObserved={observed.CancellationObserved}");

            var defect = observed.SecondOutcome == DispatchOutcome.Executed
                         && observed.SecondWorkRan
                         && observed.MaxConcurrentWork == 2
                         && observed.FirstError is SemaphoreFullException;

            var fixedContract = observed.FirstOutcome == DispatchOutcome.Executed
                                && observed.FirstError is null
                                && observed.SecondOutcome == DispatchOutcome.Busy
                                && observed.SecondError is null
                                && !observed.SecondWorkRan
                                && observed.MaxConcurrentWork == 1
                                && observed.ThirdOutcome == DispatchOutcome.Executed
                                && observed.ThirdError is null
                                && observed.WorkFailurePropagated
                                && observed.PermitRestoredAfterFailure
                                && observed.CancellationObserved;

            if (mode == "reproduce")
            {
                Console.WriteLine(defect ? "REPRODUCED: exclusive admission violated." : "NOT REPRODUCED: inspect the starter and harness.");
                return defect ? 0 : 1;
            }

            if (mode == "verify")
            {
                Console.WriteLine(fixedContract ? "PASS: admission and failure-path invariants hold." : "FAIL: at least one invariant is violated.");
                return fixedContract ? 0 : 1;
            }

            Console.WriteLine("Demo only. Use reproduce before editing and verify after editing.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"HARNESS ERROR: {ex}");
            return 2;
        }
    }

    private static async Task<Observation> ObserveAsync()
    {
        var coordinator = new DispatchCoordinator();
        var firstEntered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowFirstToFinish = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var activeWork = 0;
        var maxConcurrentWork = 0;
        var secondWorkRan = false;

        async Task FirstCommand()
        {
            var current = Interlocked.Increment(ref activeWork);
            Interlocked.Exchange(ref maxConcurrentWork, Math.Max(maxConcurrentWork, current));
            firstEntered.TrySetResult(true);
            try
            {
                await allowFirstToFinish.Task;
            }
            finally
            {
                Interlocked.Decrement(ref activeWork);
            }
        }

        var firstTask = coordinator.DispatchAsync(FirstCommand, TimeSpan.FromSeconds(2));
        await firstEntered.Task.WaitAsync(TimeSpan.FromSeconds(3));

        var second = await CaptureAsync(() => coordinator.DispatchAsync(
            () =>
            {
                secondWorkRan = true;
                var current = Interlocked.Increment(ref activeWork);
                Interlocked.Exchange(ref maxConcurrentWork, Math.Max(maxConcurrentWork, current));
                Interlocked.Decrement(ref activeWork);
                return Task.CompletedTask;
            },
            TimeSpan.Zero));

        allowFirstToFinish.TrySetResult(true);
        var first = await CaptureAsync(async () => await firstTask.WaitAsync(TimeSpan.FromSeconds(3)));
        var third = await CaptureAsync(() => coordinator.DispatchAsync(
            () => Task.CompletedTask, TimeSpan.Zero));

        var failure = await CaptureAsync(() => coordinator.DispatchAsync(
            () => Task.FromException(new InvalidOperationException("simulated command failure")),
            TimeSpan.Zero));

        var afterFailure = await CaptureAsync(() => coordinator.DispatchAsync(
            () => Task.CompletedTask, TimeSpan.Zero));

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        var cancelledAttempt = await CaptureAsync(() => coordinator.DispatchAsync(
            () => Task.CompletedTask, TimeSpan.FromSeconds(1), cancelled.Token));

        return new Observation(
            first.Outcome, first.Error,
            second.Outcome, second.Error,
            secondWorkRan, maxConcurrentWork,
            third.Outcome, third.Error,
            failure.Error is InvalidOperationException,
            afterFailure.Outcome == DispatchOutcome.Executed && afterFailure.Error is null,
            cancelledAttempt.Error is OperationCanceledException);
    }

    private static async Task<(DispatchOutcome? Outcome, Exception? Error)> CaptureAsync(
        Func<Task<DispatchOutcome>> action)
    {
        try
        {
            return (await action(), null);
        }
        catch (Exception ex)
        {
            return (null, ex);
        }
    }

    private sealed record Observation(
        DispatchOutcome? FirstOutcome,
        Exception? FirstError,
        DispatchOutcome? SecondOutcome,
        Exception? SecondError,
        bool SecondWorkRan,
        int MaxConcurrentWork,
        DispatchOutcome? ThirdOutcome,
        Exception? ThirdError,
        bool WorkFailurePropagated,
        bool PermitRestoredAfterFailure,
        bool CancellationObserved);
}
