using System;
using System.Threading;
using System.Threading.Tasks;

return await LabRunner.MainAsync(args);

internal static class LabRunner
{
    public static async Task<int> MainAsync(string[] args)
    {
        string mode = args.Length == 1 ? args[0] : "--inspect";
        if (mode is not ("--inspect" or "--reproduce" or "--verify"))
        {
            Console.Error.WriteLine("Usage: --inspect | --reproduce | --verify");
            return 2;
        }

        int count = mode == "--verify" ? 3 : 1;
        bool allCompleted = true, observedStall = false;
        for (int attempt = 1; attempt <= count; attempt++)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RunResult result = await PublishOnceAsync(TimeSpan.FromMilliseconds(1200));
            watch.Stop();
            Console.WriteLine($"attempt={attempt} status={result.Status} publisherDone={result.PublisherCompleted} subscriberDone={result.SubscriberCompleted} delivered={result.Delivered} elapsedMs={watch.ElapsedMilliseconds}");
            observedStall |= result.Status == "STALLED" && result.Delivered == 0
                && !result.PublisherCompleted && !result.SubscriberCompleted;
            allCompleted &= result.Status == "COMPLETED" && result.Delivered == 1
                && result.PublisherCompleted && result.SubscriberCompleted;
        }
        if (mode == "--reproduce")
        {
            Console.WriteLine(observedStall ? "REPRODUCED" : "NOT_REPRODUCED");
            return observedStall ? 0 : 1;
        }
        if (mode == "--verify")
        {
            Console.WriteLine(allCompleted ? "VERIFIED" : "NOT_VERIFIED");
            return allCompleted ? 0 : 1;
        }
        return 0;
    }

    private static async Task<RunResult> PublishOnceAsync(TimeSpan deadline)
    {
        var gate = new SemaphoreSlim(1, 1);
        var published = new TaskCompletionSource<bool>();
        int delivered = 0;

        async Task SubscriberAsync()
        {
            await published.Task.ConfigureAwait(false);
            gate.Wait();
            try { delivered++; }
            finally { gate.Release(); }
        }

        Task subscriber = SubscriberAsync();
        Task publisher = Task.Run(async () =>
        {
            await gate.WaitAsync().ConfigureAwait(false);
            try { published.TrySetResult(true); }
            finally { gate.Release(); }
        });

        Task both = Task.WhenAll(subscriber, publisher);
        Task winner = await Task.WhenAny(both, Task.Delay(deadline)).ConfigureAwait(false);
        if (winner != both)
            return new RunResult("STALLED", delivered, publisher.IsCompleted, subscriber.IsCompleted);
        try
        {
            await both.ConfigureAwait(false);
            return new RunResult("COMPLETED", delivered, publisher.IsCompleted, subscriber.IsCompleted);
        }
        catch (Exception ex)
        {
            return new RunResult("FAULT:" + ex.GetType().Name, delivered, publisher.IsCompleted, subscriber.IsCompleted);
        }
    }

    private sealed record RunResult(string Status, int Delivered, bool PublisherCompleted, bool SubscriberCompleted);
}
