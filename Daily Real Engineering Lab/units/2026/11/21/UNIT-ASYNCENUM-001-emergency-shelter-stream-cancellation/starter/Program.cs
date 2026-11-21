using ShelterLab;

static async Task<int> ProbeAsync(bool expectCancellation)
{
    using var cts = new CancellationTokenSource();
    var received = new List<int>();
    bool canceled = false;
    try
    {
        await foreach (var item in ShelterFeed.GetCapacityUpdatesAsync().WithCancellation(cts.Token))
        {
            received.Add(item);
            if (received.Count == 1) cts.Cancel();
        }
    }
    catch (OperationCanceledException) when (cts.IsCancellationRequested)
    {
        canceled = true;
    }

    Console.WriteLine($"received={string.Join(",", received)} canceled={canceled}");
    if (expectCancellation)
    {
        Console.WriteLine(canceled && received.SequenceEqual(new[] { 1 })
            ? "CANCEL_OBSERVED" : "CANCEL_IGNORED");
        return canceled && received.SequenceEqual(new[] { 1 }) ? 0 : 1;
    }
    return !canceled && received.SequenceEqual(new[] { 1, 2, 3 }) ? 0 : 1;
}

static async Task<int> NormalAsync()
{
    var values = new List<int>();
    await foreach (var item in ShelterFeed.GetCapacityUpdatesAsync()) values.Add(item);
    var ok = values.SequenceEqual(new[] { 1, 2, 3 });
    Console.WriteLine($"normal={string.Join(",", values)} result={(ok ? "PASS" : "FAIL")}");
    return ok ? 0 : 1;
}

var mode = args.FirstOrDefault() ?? "normal";
switch (mode)
{
    case "normal": return await NormalAsync();
    case "probe": return await ProbeAsync(expectCancellation: true);
    case "reproduce":
        // The starter is expected to ignore cancellation. After a fix, this mode fails intentionally.
        var observed = await ProbeAsync(expectCancellation: true);
        return observed == 1 ? 0 : 1;
    case "verify":
        var a = await ProbeAsync(expectCancellation: true);
        var b = await NormalAsync();
        return a == 0 && b == 0 ? 0 : 1;
    default:
        Console.Error.WriteLine("Modes: normal, probe, reproduce, verify");
        return 2;
}
