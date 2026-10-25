using SeatFeed;

var mode = args.Length == 2 && args[0] == "--mode" ? args[1] : "observe";
try
{
    var normal = await NormalAsync();
    var direct = await DirectTokenAsync();
    var afterFirst = await SubscriberTokenAsync(false);
    var beforeFirst = await SubscriberTokenAsync(true);
    Console.WriteLine($"normal_stream_count={normal}");
    Console.WriteLine($"direct_parameter_canceled={direct}");
    Console.WriteLine($"subscriber_canceled_after_first={afterFirst}");
    Console.WriteLine($"subscriber_canceled_before_first={beforeFirst}");

    if (mode == "observe") return 0;
    if (mode == "reproduce")
    {
        var reproduced = normal == 3 && direct && !afterFirst && !beforeFirst;
        Console.WriteLine(reproduced ? "REPRODUCED: subscriber cancellation ignored." : "NOT REPRODUCED.");
        return reproduced ? 0 : 1;
    }
    if (mode == "verify")
    {
        var passed = normal == 3 && direct && afterFirst && beforeFirst;
        Console.WriteLine(passed ? "PASS: streaming contracts hold." : "FAIL: cancellation or normal stream regression.");
        return passed ? 0 : 1;
    }
    Console.Error.WriteLine("Unknown mode; expected observe, reproduce or verify.");
    return 2;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected failure: {ex}");
    return 2;
}

static async Task<int> NormalAsync()
{
    var count = 0;
    await foreach (var update in SeatAvailabilityFeed.WatchAsync())
    {
        if (string.IsNullOrWhiteSpace(update.SeatId))
            throw new InvalidOperationException("Invalid seat update.");
        count++;
    }
    return count;
}

static async Task<bool> DirectTokenAsync()
{
    using var source = new CancellationTokenSource();
    await using var enumerator = SeatAvailabilityFeed.WatchAsync(source.Token).GetAsyncEnumerator();
    if (!await enumerator.MoveNextAsync()) throw new InvalidOperationException("No first update.");
    source.Cancel();
    try { await enumerator.MoveNextAsync(); return false; }
    catch (OperationCanceledException) { return true; }
}

static async Task<bool> SubscriberTokenAsync(bool cancelBeforeFirst)
{
    using var source = new CancellationTokenSource();
    await using var enumerator = SeatAvailabilityFeed.WatchAsync()
        .WithCancellation(source.Token).GetAsyncEnumerator();
    if (!cancelBeforeFirst && !await enumerator.MoveNextAsync())
        throw new InvalidOperationException("No first update.");
    source.Cancel();
    try { await enumerator.MoveNextAsync(); return false; }
    catch (OperationCanceledException) { return true; }
}
