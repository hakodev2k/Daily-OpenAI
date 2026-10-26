using AdWindowLab;

return args.FirstOrDefault()?.ToLowerInvariant() switch
{
    "verify" => await VerifyAsync(),
    "reproduce" or null => await ReproduceAsync(),
    _ => Usage()
};

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run -- reproduce|verify");
    return 2;
}

static async Task<int> ReproduceAsync()
{
    // A short period represents one accounting hour in the timer demonstration.
    using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(80));
    await timer.WaitForNextTickAsync();
    await Task.Delay(480);
    await timer.WaitForNextTickAsync();

    var origin = new DateTimeOffset(2026, 10, 27, 0, 0, 0, TimeSpan.FromHours(7));
    var scheduler = new WindowScheduler(origin, TimeSpan.FromHours(1));
    var sink = new RecordingSink();
    var processed = await scheduler.OnWakeupAsync(origin.AddHours(5), sink);

    Console.WriteLine("Elapsed logical windows: 5");
    Console.WriteLine($"Processed on wake-up: {processed}");
    Console.WriteLine($"Last completed: {scheduler.LastCompletedWindow:O}");
    Console.WriteLine($"Recorded: {string.Join(", ", sink.Completed.Select(x => x.ToString("HH:mm")))}");

    if (processed == 1 && scheduler.LastCompletedWindow == origin.AddHours(1))
    {
        Console.WriteLine("REPRODUCED: the worker is behind the required business windows.");
        return 0;
    }

    Console.Error.WriteLine("Original symptom not observed; inspect starter changes.");
    return 1;
}

static async Task<int> VerifyAsync()
{
    try
    {
        var origin = new DateTimeOffset(2026, 10, 27, 0, 0, 0, TimeSpan.FromHours(7));
        var now = origin.AddHours(5);
        var expected = Enumerable.Range(1, 5).Select(i => origin.AddHours(i)).ToArray();

        var scheduler = new WindowScheduler(origin, TimeSpan.FromHours(1));
        var sink = new RecordingSink();
        var count = await scheduler.OnWakeupAsync(now, sink);
        Check(count == 5, $"Expected 5 completed windows, got {count}.");
        Check(sink.Completed.SequenceEqual(expected), "Window order or coverage is incorrect.");
        Check(scheduler.LastCompletedWindow == now, "Checkpoint must match last completed window.");

        var noOp = await scheduler.OnWakeupAsync(now, sink);
        Check(noOp == 0 && sink.Completed.Count == 5, "Repeated wake-up must not replay completed windows.");

        var recovering = new WindowScheduler(origin, TimeSpan.FromHours(1));
        var failedSink = new RecordingSink(origin.AddHours(3));
        var failureSeen = false;
        try
        {
            await recovering.OnWakeupAsync(now, failedSink);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Injected downstream failure")
        {
            failureSeen = true;
        }

        Check(failureSeen, "Expected an injected failure on the third window.");
        Check(recovering.LastCompletedWindow == origin.AddHours(2),
            "Checkpoint advanced beyond an uncompleted window.");
        Check(failedSink.Completed.SequenceEqual(expected.Take(2)),
            "Unexpected processing before the injected failure.");

        var retrySink = new RecordingSink();
        var retryCount = await recovering.OnWakeupAsync(now, retrySink);
        Check(retryCount == 3, $"Expected 3 remaining windows after retry, got {retryCount}.");
        Check(retrySink.Completed.SequenceEqual(expected.Skip(2)),
            "Retry must resume at the first uncompleted window.");
        Check(recovering.LastCompletedWindow == now, "Retry did not reach the current window.");

        Console.WriteLine("PASS: coverage, ordering, idempotent wake-up, failure checkpoint and recovery.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"FAIL: {ex.Message}");
        return 1;
    }
}

static void Check(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

sealed class RecordingSink(DateTimeOffset? failAt = null) : IWindowSink
{
    public List<DateTimeOffset> Completed { get; } = new();

    public Task ProcessAsync(DateTimeOffset windowEnd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (failAt == windowEnd)
            throw new InvalidOperationException("Injected downstream failure");

        Completed.Add(windowEnd);
        return Task.CompletedTask;
    }
}
