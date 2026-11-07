using OrbitTileLab;

return await LabScenario.MainAsync(args);

internal static class LabScenario
{
    private sealed record CaseResult(
        bool QueuedRequestCancelled,
        bool CancelledRequestExecuted,
        bool ThirdEnteredBeforeFirstReleased,
        string? FirstFault,
        string? ThirdFault);

    public static async Task<int> MainAsync(string[] args)
    {
        string mode = args.Length == 0 ? "observe" : args[0].ToLowerInvariant();
        if (mode is not ("observe" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [observe|reproduce|verify]");
            return 2;
        }

        try
        {
            CaseResult incident = await RunCancelledWaiterCaseAsync();
            bool healthyQueue = await RunHealthyQueueCaseAsync();
            bool acquiredCancellation = await RunCancelAfterAcquireCaseAsync();

            Console.WriteLine($"queued_cancelled={incident.QueuedRequestCancelled}");
            Console.WriteLine($"cancelled_work_executed={incident.CancelledRequestExecuted}");
            Console.WriteLine($"third_entered_before_first_released={incident.ThirdEnteredBeforeFirstReleased}");
            Console.WriteLine($"first_fault={incident.FirstFault ?? "none"}");
            Console.WriteLine($"third_fault={incident.ThirdFault ?? "none"}");
            Console.WriteLine($"healthy_queue_contract={healthyQueue}");
            Console.WriteLine($"cancel_after_acquire_contract={acquiredCancellation}");

            bool common = incident.QueuedRequestCancelled
                && !incident.CancelledRequestExecuted && healthyQueue && acquiredCancellation;
            bool observedFailure = common && incident.ThirdEnteredBeforeFirstReleased
                && incident.FirstFault is null
                && incident.ThirdFault == nameof(SemaphoreFullException);
            bool repaired = common && !incident.ThirdEnteredBeforeFirstReleased
                && incident.FirstFault is null && incident.ThirdFault is null;

            if (mode == "reproduce")
            {
                Console.WriteLine(observedFailure ? "REPRODUCED: capacity invariant violated" : "NOT REPRODUCED");
                return observedFailure ? 0 : 1;
            }

            if (mode == "verify")
            {
                Console.WriteLine(repaired ? "VERIFIED: cancellation and capacity contracts pass" : "VERIFY FAILED");
                return repaired ? 0 : 1;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"HARNESS ERROR: {ex}");
            return 2;
        }
    }

    private static TaskCompletionSource<bool> Signal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static async Task<string?> FaultNameAsync(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
            return null;
        }
        catch (Exception ex)
        {
            return ex.GetType().Name;
        }
    }

    private static async Task<CaseResult> RunCancelledWaiterCaseAsync()
    {
        var gate = new TileGate();
        var firstEntered = Signal();
        var releaseFirst = Signal();
        var thirdEntered = Signal();
        var releaseThird = Signal();

        Task first = gate.RenderAsync("sat-A", async _ =>
        {
            firstEntered.TrySetResult(true);
            await releaseFirst.Task.ConfigureAwait(false);
        });
        await firstEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        using var cancelQueued = new CancellationTokenSource();
        bool cancelledWorkExecuted = false;
        Task second = gate.RenderAsync("sat-B", _ =>
        {
            cancelledWorkExecuted = true;
            return Task.CompletedTask;
        }, cancelQueued.Token);

        cancelQueued.Cancel();
        bool queuedCancelled = false;
        try
        {
            await second.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            queuedCancelled = true;
        }

        Task third = gate.RenderAsync("sat-C", async _ =>
        {
            thirdEntered.TrySetResult(true);
            await releaseThird.Task.ConfigureAwait(false);
        });

        // This check occurs before the first job's release signal, not after a delay.
        bool enteredEarly = thirdEntered.Task.IsCompleted;
        releaseFirst.TrySetResult(true);
        string? firstFault = await FaultNameAsync(first);
        await thirdEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        releaseThird.TrySetResult(true);
        string? thirdFault = await FaultNameAsync(third);

        return new CaseResult(queuedCancelled, cancelledWorkExecuted, enteredEarly, firstFault, thirdFault);
    }

    private static async Task<bool> RunHealthyQueueCaseAsync()
    {
        var gate = new TileGate();
        var entered = Signal();
        var release = Signal();
        bool secondEntered = false;

        Task first = gate.RenderAsync("healthy-A", async _ =>
        {
            entered.TrySetResult(true);
            await release.Task.ConfigureAwait(false);
        });
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Task second = gate.RenderAsync("healthy-B", _ =>
        {
            secondEntered = true;
            return Task.CompletedTask;
        });
        bool waited = !secondEntered;
        release.TrySetResult(true);
        await Task.WhenAll(first, second).WaitAsync(TimeSpan.FromSeconds(5));
        return waited && secondEntered;
    }

    private static async Task<bool> RunCancelAfterAcquireCaseAsync()
    {
        var gate = new TileGate();
        using var cts = new CancellationTokenSource();
        bool cancelled = false;
        try
        {
            await gate.RenderAsync("cancel-in-render", _ =>
            {
                cts.Cancel();
                return Task.FromCanceled(cts.Token);
            }, cts.Token);
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
        }

        bool nextEntered = false;
        await gate.RenderAsync("after-cancel", _ =>
        {
            nextEntered = true;
            return Task.CompletedTask;
        }).WaitAsync(TimeSpan.FromSeconds(5));

        return cancelled && nextEntered;
    }
}
