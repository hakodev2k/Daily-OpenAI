namespace BadgeDrainLab;

internal static class Program
{
    private readonly record struct Scenario(JobSnapshot BeforeRetry, JobSnapshot AfterRetry);

    private static async Task<int> Main(string[] args)
    {
        if (args.Length != 1 || args[0] is not ("observe" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [observe|reproduce|verify]");
            return 64;
        }

        var cancelled = await CancelledDuringRenderAsync();
        var receipt = await ReceiptFailureAfterPublishAsync();

        Console.WriteLine($"CANCELLED beforeRetry={cancelled.BeforeRetry} afterRetry={cancelled.AfterRetry}");
        Console.WriteLine($"RECEIPT   beforeRetry={receipt.BeforeRetry} afterRetry={receipt.AfterRetry}");

        if (args[0] == "observe") return 0;

        var baseline =
            cancelled.BeforeRetry == new JobSnapshot(0, 0, 1, 0, 0) &&
            cancelled.AfterRetry  == new JobSnapshot(0, 0, 1, 0, 0) &&
            receipt.BeforeRetry   == new JobSnapshot(0, 0, 1, 1, 1) &&
            receipt.AfterRetry    == new JobSnapshot(0, 0, 1, 1, 1);

        var repaired =
            cancelled.BeforeRetry == new JobSnapshot(1, 0, 0, 0, 0) &&
            cancelled.AfterRetry  == new JobSnapshot(0, 0, 1, 1, 1) &&
            receipt.BeforeRetry   == new JobSnapshot(1, 0, 0, 1, 1) &&
            receipt.AfterRetry    == new JobSnapshot(0, 0, 1, 1, 2);

        if (args[0] == "reproduce" && baseline)
        {
            Console.WriteLine("EXPECTED_FAILURE_REPRODUCED");
            return 0;
        }
        if (args[0] == "verify" && repaired)
        {
            Console.WriteLine("VERIFICATION_PASSED");
            return 0;
        }

        Console.Error.WriteLine(args[0] == "reproduce"
            ? "REPRODUCTION_NOT_CONFIRMED"
            : "VERIFICATION_FAILED");
        return 1;
    }

    private static async Task<Scenario> CancelledDuringRenderAsync()
    {
        var store = new JobStore("BADGE-042");
        var started = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var hold = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        using var shutdown = new CancellationTokenSource();

        var renderer = new BadgeRenderer(store, hold.Task, started: started);
        var running = BadgeWorker.HandleOneAsync(store, renderer, shutdown.Token);
        await started.Task;
        shutdown.Cancel();

        try { await running; }
        catch (OperationCanceledException) when (shutdown.IsCancellationRequested) { }

        var before = store.Snapshot();
        await BadgeWorker.HandleOneAsync(
            store, new BadgeRenderer(store, Task.CompletedTask), CancellationToken.None);
        var after = store.Snapshot();
        return new Scenario(before, after);
    }

    private static async Task<Scenario> ReceiptFailureAfterPublishAsync()
    {
        var store = new JobStore("BADGE-077");
        try
        {
            await BadgeWorker.HandleOneAsync(
                store,
                new BadgeRenderer(store, Task.CompletedTask, failAfterPublish: true),
                CancellationToken.None);
        }
        catch (IOException) { /* Controlled injected failure. */ }

        var before = store.Snapshot();
        await BadgeWorker.HandleOneAsync(
            store, new BadgeRenderer(store, Task.CompletedTask), CancellationToken.None);
        var after = store.Snapshot();
        return new Scenario(before, after);
    }
}
