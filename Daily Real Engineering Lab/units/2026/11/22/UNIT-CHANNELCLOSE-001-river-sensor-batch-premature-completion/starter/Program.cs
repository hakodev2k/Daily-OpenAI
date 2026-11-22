using System.Threading.Channels;

var verify = args.Contains("--verify", StringComparer.OrdinalIgnoreCase);
const int stations = 3, perStation = 4;
var channel = Channel.CreateUnbounded<Reading>(new UnboundedChannelOptions
{
    SingleReader = true, SingleWriter = false
});
var accepted = new List<Reading>();
var producerErrors = new List<Exception>();
var producers = Enumerable.Range(1, stations).Select(station => Task.Run(async () =>
{
    try
    {
        for (var n = 1; n <= perStation; n++)
        {
            await Task.Delay(station * 8);
            await channel.Writer.WriteAsync(new Reading(station, n));
        }
    }
    catch (Exception ex)
    {
        lock (producerErrors) producerErrors.Add(ex);
    }
})).ToArray();

var consumer = Task.Run(async () =>
{
    await foreach (var reading in channel.Reader.ReadAllAsync())
        accepted.Add(reading);
});

// Batch coordination was extracted during the last gateway migration.
await Task.WhenAny(producers);
channel.Writer.TryComplete();
await Task.WhenAll(producers);
await consumer;
var expected = stations * perStation;
var unique = accepted.Select(x => (x.Station, x.Sequence)).Distinct().Count();
Console.WriteLine($"expected={expected} received={accepted.Count} unique={unique} missing={expected - unique} producerErrors={producerErrors.Count}");
if (verify && (accepted.Count != expected || unique != expected || producerErrors.Count != 0))
    Environment.ExitCode = 1;

record Reading(int Station, int Sequence);
