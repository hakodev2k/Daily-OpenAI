using RunwayLighting;
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            return (args.FirstOrDefault() ?? "demo") switch
            {
                "demo" => await Run("demo"),
                "reproduce" => await Run("reproduce"),
                "verify" => await Run("verify"),
                _ => throw new ArgumentException("Mode: demo, reproduce, verify")
            };
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 2; }
    }
    private static async Task<int> Run(string mode)
    {
        using var source = new CancellationTokenSource();
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var coordinator = new ShutdownCoordinator(source.Token, gate.Task);
        Task cancellation = Task.Run(() => source.Cancel());
        await coordinator.CallbackEntered.WaitAsync(TimeSpan.FromSeconds(5));
        int observationMs = mode == "verify" ? 1500 : 350;
        Task first = await Task.WhenAny(cancellation, Task.Delay(observationMs));
        bool returnedEarly = ReferenceEquals(first, cancellation);
        Console.WriteLine($"Cancel returned while gate closed: {returnedEarly}");
        Console.WriteLine($"Flush count before gate open: {coordinator.FlushCount}");
        bool flushTooEarly = coordinator.CleanupCompleted.IsCompleted || coordinator.FlushCount != 0;
        gate.TrySetResult(true);
        await cancellation.WaitAsync(TimeSpan.FromSeconds(5));
        await coordinator.CleanupCompleted.WaitAsync(TimeSpan.FromSeconds(5));
        if (flushTooEarly || coordinator.FlushCount != 1)
            throw new InvalidOperationException("Cleanup contract violated.");
        if (mode == "reproduce")
        {
            if (returnedEarly) throw new InvalidOperationException("Symptom no longer reproduces; run verify.");
            Console.WriteLine("REPRODUCED: cancellation waits for downstream gate.");
            return 0;
        }
        if (mode == "verify")
        {
            if (!returnedEarly) throw new InvalidOperationException("FAIL: cancellation acknowledgement waits for downstream.");
            source.Cancel();
            if (coordinator.FlushCount != 1) throw new InvalidOperationException("FAIL: duplicate cleanup.");
            using var alreadyStopped = new CancellationTokenSource();
            alreadyStopped.Cancel();
            using var second = new ShutdownCoordinator(alreadyStopped.Token, Task.CompletedTask);
            await second.CallbackEntered.WaitAsync(TimeSpan.FromSeconds(5));
            await second.CleanupCompleted.WaitAsync(TimeSpan.FromSeconds(5));
            if (second.FlushCount != 1) throw new InvalidOperationException("FAIL: pre-cancelled token.");
            Console.WriteLine("PASS: prompt acknowledgement, owned cleanup, exactly once, pre-cancelled token.");
        }
        return 0;
    }
}
