const int count = 1000;
const int baselineKeepEvery = 20;
const int diagnosticThresholdMs = 1000;
var all = new List<int>();
var kept = new List<int>();
for (var i = 1; i <= count; i++)
{
    var latency = i % 97 == 0 ? 1200 : 40 + (i % 25);
    var baselineKeep = i % baselineKeepEvery == 0;
    var diagnosticKeep = latency >= diagnosticThresholdMs;
    all.Add(latency);
    if (baselineKeep || diagnosticKeep) kept.Add(latency);
}
static int P(List<int> values, double p)
{
    var sorted = values.Order().ToArray();
    return sorted[(int)Math.Ceiling(sorted.Length * p) - 1];
}
Console.WriteLine($"requests={all.Count} traces={kept.Count}");
Console.WriteLine($"all p50={P(all,.50)}ms p95={P(all,.95)}ms p99={P(all,.99)}ms max={all.Max()}ms");
Console.WriteLine($"kept p50={P(kept,.50)}ms p95={P(kept,.95)}ms p99={P(kept,.99)}ms max={kept.Max()}ms");
Console.WriteLine($"slow-all={all.Count(x => x >= diagnosticThresholdMs)} slow-kept={kept.Count(x => x >= diagnosticThresholdMs)}");