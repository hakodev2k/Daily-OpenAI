using System.Globalization;

var requests = GetIntArg(args, "--requests", 2_000);
var sink = new MetricSink();
var telemetry = new CheckoutTelemetry(sink);

for (var i = 0; i < requests; i++)
{
    var userId = $"user-{i % 1500:D4}";
    var region = i % 2 == 0 ? "ap-southeast" : "eu-west";
    var statusCode = i % 20 == 0 ? 500 : 200;
    var durationMs = 80 + (i % 17);

    telemetry.RecordCheckout(userId, region, statusCode, durationMs);
}

Console.WriteLine($"requests={requests}");
Console.WriteLine($"measurements={sink.MeasurementCount}");
Console.WriteLine($"series_count={sink.SeriesCount}");
Console.WriteLine($"dimension_names={string.Join(',', sink.DimensionNames.OrderBy(x => x, StringComparer.Ordinal))}");

static int GetIntArg(string[] args, string name, int fallback)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length && int.TryParse(args[index + 1], out var value)
        ? value
        : fallback;
}

sealed class CheckoutTelemetry(MetricSink sink)
{
    public void RecordCheckout(string userId, string region, int statusCode, double durationMs)
    {
        // Investigation note: which dimensions should define an aggregatable time series?
        sink.Record(
            durationMs,
            new("region", region),
            new("status_code", statusCode.ToString(CultureInfo.InvariantCulture)),
            new("user_id", userId));
    }
}

sealed class MetricSink
{
    private readonly HashSet<string> _series = new(StringComparer.Ordinal);
    private readonly HashSet<string> _dimensionNames = new(StringComparer.Ordinal);

    public long MeasurementCount { get; private set; }
    public int SeriesCount => _series.Count;
    public IReadOnlyCollection<string> DimensionNames => _dimensionNames;

    public void Record(double value, params KeyValuePair<string, string>[] dimensions)
    {
        _ = value;
        MeasurementCount++;

        foreach (var dimension in dimensions)
        {
            _dimensionNames.Add(dimension.Key);
        }

        var key = string.Join(
            "|",
            dimensions
                .OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => $"{x.Key}={x.Value}"));

        _series.Add(key);
    }
}
