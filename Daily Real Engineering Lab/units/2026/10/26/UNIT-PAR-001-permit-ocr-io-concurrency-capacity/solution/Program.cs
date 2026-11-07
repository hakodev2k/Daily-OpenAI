using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

const int documentCount = 24;
const int partnerCapacity = 8;
var requested = 8;
for (var i = 0; i < args.Length; i++)
{
    if (args[i] == "--max-concurrency" && i + 1 < args.Length)
        requested = int.Parse(args[++i]);
}
if (requested < 1 || requested > 64)
    throw new ArgumentOutOfRangeException(nameof(requested), "Allowed: 1..64.");

var maxParallel = Math.Clamp(requested, 1, partnerCapacity);
var processed = new ConcurrentDictionary<int, int>();
var active = 0;
var peak = 0;
var rejected = 0;
var watch = Stopwatch.StartNew();

await Parallel.ForEachAsync(Enumerable.Range(1, documentCount), new ParallelOptions
{
    MaxDegreeOfParallelism = maxParallel
}, async (documentId, token) =>
{
    var concurrent = Interlocked.Increment(ref active);
    UpdatePeak(ref peak, concurrent);
    try
    {
        if (concurrent > partnerCapacity)
        {
            Interlocked.Increment(ref rejected);
            await Task.Delay(10, token);
            return;
        }
        await Task.Delay(120, token);
        processed.AddOrUpdate(documentId, 1, (_, previous) => previous + 1);
    }
    finally { Interlocked.Decrement(ref active); }
});
watch.Stop();

Console.WriteLine(JsonSerializer.Serialize(new
{
    requestedConcurrency = requested,
    effectiveConcurrency = maxParallel,
    maxActive = peak,
    processedCount = processed.Count,
    duplicateCount = processed.Values.Count(v => v > 1),
    rejectedCount = rejected,
    missingCount = documentCount - processed.Count,
    elapsedMs = watch.ElapsedMilliseconds
}, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

static void UpdatePeak(ref int peak, int current)
{
    while (true)
    {
        var previous = Volatile.Read(ref peak);
        if (current <= previous) return;
        if (Interlocked.CompareExchange(ref peak, current, previous) == previous) return;
    }
}
