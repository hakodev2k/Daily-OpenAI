using System.Collections.Concurrent;

var mode = args.FirstOrDefault() ?? "reproduce";
var registry = new MetricRegistry();

for (var i = 1; i <= 5000; i++)
{
    var userId = $"user-{i:00000}";
    var path = $"/api/orders/{userId}";
    registry.Record(path, userId, 200);
}

Console.WriteLine($"Total requests: {registry.TotalRequests}");
Console.WriteLine($"Metric series: {registry.SeriesCount}");
foreach (var key in registry.SampleSeries(5)) Console.WriteLine(key);

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    if (registry.TotalRequests != 5000) return 2;
    if (registry.SeriesCount > 10) return 3;
}
else
{
    if (registry.SeriesCount < 1000) return 4;
}

return 0;

sealed class MetricRegistry
{
    private readonly ConcurrentDictionary<string, long> _series = new();
    public long TotalRequests { get; private set; }
    public int SeriesCount => _series.Count;

    public void Record(string requestPath, string userId, int statusCode)
    {
        var tags = BuildTags(requestPath, userId, statusCode);
        _series.AddOrUpdate(tags, 1, static (_, current) => current + 1);
        TotalRequests++;
    }

    private static string BuildTags(string requestPath, string userId, int statusCode)
        => $"path={requestPath}|user={userId}|status={statusCode}";

    public IEnumerable<string> SampleSeries(int count) => _series.Keys.Take(count);
}
