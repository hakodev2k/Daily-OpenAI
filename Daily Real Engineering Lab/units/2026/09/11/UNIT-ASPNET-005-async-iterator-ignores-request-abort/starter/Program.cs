var mode = args.FirstOrDefault() ?? "run";
var pool = new FakeConnectionPool(capacity: 2);
var streamer = new AuditStreamer(pool);

const int consumerCount = 6;
var cancellationSources = Enumerable.Range(0, consumerCount)
    .Select(_ => new CancellationTokenSource())
    .ToArray();

var consumers = cancellationSources
    .Select(cts => ConsumeAsync(streamer, cts.Token))
    .ToArray();

await Task.Delay(250);
foreach (var cts in cancellationSources)
{
    cts.Cancel();
}

Console.WriteLine($"CLIENT_CANCELLATIONS_SENT={consumerCount}");

await Task.Delay(500);

var activeAfterGrace = streamer.ActiveProducers;
var inUseAfterGrace = pool.InUse;
var waitingAfterGrace = pool.Waiting;

Console.WriteLine($"ACTIVE_PRODUCERS_AFTER_GRACE={activeAfterGrace}");
Console.WriteLine($"POOL_IN_USE_AFTER_GRACE={inUseAfterGrace}");
Console.WriteLine($"WAITING_FOR_POOL_AFTER_GRACE={waitingAfterGrace}");

var incidentStillPresent =
    activeAfterGrace > 0 ||
    inUseAfterGrace > 0 ||
    waitingAfterGrace > 0;

if (mode.Equals("reproduce", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine(incidentStillPresent
        ? "RESULT=INCIDENT_REPRODUCED"
        : "RESULT=INCIDENT_NOT_REPRODUCED");
}
else if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine(incidentStillPresent
        ? "RESULT=FIX_NOT_VERIFIED"
        : "RESULT=FIX_VERIFIED");
}
else
{
    Console.WriteLine(incidentStillPresent
        ? "RESULT=RESOURCE_RETENTION_OBSERVED"
        : "RESULT=RESOURCE_RELEASE_OBSERVED");
}

await Task.WhenAll(consumers);
foreach (var cts in cancellationSources)
{
    cts.Dispose();
}

return mode.ToLowerInvariant() switch
{
    "reproduce" => incidentStillPresent ? 0 : 1,
    "verify" => incidentStillPresent ? 1 : 0,
    _ => 0
};

static async Task ConsumeAsync(AuditStreamer streamer, CancellationToken cancellationToken)
{
    try
    {
        await foreach (var _ in streamer.StreamAsync().WithCancellation(cancellationToken))
        {
            // Simulates writing each item to a response stream.
        }
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
        // A canceled client is expected in this simulation.
    }
}