using System.Collections.Concurrent;
using QueueLab;

internal static class Program
{
    private const int Total = 80;

    private static async Task<int> Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- reproduce|verify");
            return 2;
        }

        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var processed = new ConcurrentBag<int>();

        await using var dispatcher = new DispatchQueue(async item =>
        {
            started.TrySetResult(true);
            await release.Task;
            processed.Add(item);
        });

        var publishes = Enumerable.Range(1, Total)
            .Select(i => dispatcher.EnqueueAsync(i).AsTask())
            .ToArray();
        var allPublished = Task.WhenAll(publishes);

        bool allAcceptedBeforeRelease;
        long acceptedBeforeRelease;
        long peakBeforeRelease;
        try
        {
            await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            allAcceptedBeforeRelease = allPublished.IsCompletedSuccessfully;
            acceptedBeforeRelease = dispatcher.Accepted;
            peakBeforeRelease = dispatcher.PeakOutstanding;
        }
        finally
        {
            release.TrySetResult(true);
        }

        await allPublished.WaitAsync(TimeSpan.FromSeconds(10));
        await dispatcher.CompleteAsync().WaitAsync(TimeSpan.FromSeconds(10));

        var values = processed.ToArray();
        var noLoss = values.Length == Total
            && values.Distinct().Count() == Total
            && values.All(x => x >= 1 && x <= Total);

        Console.WriteLine($"Mode={args[0]}");
        Console.WriteLine($"AcceptedBeforeRelease={acceptedBeforeRelease}/{Total}");
        Console.WriteLine($"AllPublishedBeforeRelease={allAcceptedBeforeRelease}");
        Console.WriteLine($"PeakBeforeRelease={peakBeforeRelease}");
        Console.WriteLine($"PeakOutstanding={dispatcher.PeakOutstanding}");
        Console.WriteLine($"Accepted={dispatcher.Accepted}; Completed={dispatcher.Completed}");
        Console.WriteLine($"AllMessagesProcessedExactlyOnce={noLoss}");

        if (!noLoss || dispatcher.Accepted != Total || dispatcher.Completed != Total)
        {
            Console.Error.WriteLine("FAIL: end-to-end delivery or drain invariant.");
            return 1;
        }

        if (args[0] == "reproduce")
        {
            if (allAcceptedBeforeRelease && acceptedBeforeRelease == Total)
            {
                Console.WriteLine("REPRODUCED: entire burst accepted while downstream is held.");
                return 0;
            }

            Console.Error.WriteLine("NOT REPRODUCED: expected original symptom is absent.");
            return 1;
        }

        if (!allAcceptedBeforeRelease
            && acceptedBeforeRelease <= 18
            && dispatcher.PeakOutstanding <= 18)
        {
            Console.WriteLine("VERIFIED: admission is limited, producers wait, and all work drains.");
            return 0;
        }

        Console.Error.WriteLine("FAIL: capacity/backpressure contract is not met.");
        return 1;
    }
}
