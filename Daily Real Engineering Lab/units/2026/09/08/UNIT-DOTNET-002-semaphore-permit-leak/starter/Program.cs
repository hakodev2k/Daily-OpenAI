var gate = new SemaphoreSlim(3, 3);

async Task ProcessAsync(string item, bool downstreamFails, CancellationToken cancellationToken)
{
    await gate.WaitAsync(cancellationToken);
    Console.WriteLine($"START item={item} permits={gate.CurrentCount}");

    await Task.Delay(50, cancellationToken);

    if (downstreamFails)
    {
        Console.WriteLine($"DOWNSTREAM_FAIL item={item} permits={gate.CurrentCount}");
        throw new InvalidOperationException($"Downstream failed for {item}");
    }

    Console.WriteLine($"DONE item={item} permits={gate.CurrentCount}");
    gate.Release();
}

var failing = Enumerable.Range(1, 3)
    .Select(i => ProcessAsync($"bad-{i}", downstreamFails: true, CancellationToken.None))
    .ToArray();

foreach (var task in failing)
{
    try
    {
        await task;
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"OBSERVED={ex.Message}");
    }
}

Console.WriteLine($"AFTER_FAILURES permits={gate.CurrentCount}");

using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(400));

try
{
    await ProcessAsync("healthy-after-failures", downstreamFails: false, timeout.Token);
    Console.WriteLine("HEALTHY_RESULT=completed");
}
catch (OperationCanceledException)
{
    Console.WriteLine("HEALTHY_RESULT=timed-out-waiting-for-capacity");
}

Console.WriteLine($"FINAL permits={gate.CurrentCount}");
