const int count = 1000;
const int keepEvery = 20;
var all = new List<int>();
var kept = new List<int>();
for (var i = 1; i <= count; i++)
{
    // Deterministic traffic: rare requests become slow only after the sampling decision point.
    var keep = i % keepEvery == 0;
    var latency = i % 97 == 0 ? 1200 : 40 + (i % 25);
    all.Add(latency);
    if (keep) kept.Add(latency);
}
static int P(List<int> values, double p)
{
    var sorted = values.Order().ToArray();
    return sorted[(int)Math.Ceiling(sorted.Length * p) - 1];
}
Console.WriteLine($"requests={all.Count} traces={kept.Count}");
Console.WriteLine($"all p50={P(all,.50)}ms p95={P(all,.95)}ms p99={P(all,.99)}ms max={all.Max()}ms");
Console.WriteLine($"kept p50={P(kept,.50)}ms p95={P(kept,.95)}ms p99={P(kept,.99)}ms max={kept.Max()}ms");
Console.WriteLine($"slow-all={all.Count(x => x >= 1000)} slow-kept={kept.Count(x => x >= 1000)}");