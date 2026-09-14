using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

const string meterName = "DailyOpenAI.SearchApi";
using var meter = new Meter(meterName, "1.0.0");
var requestDuration = meter.CreateHistogram<double>("search.request.duration", unit: "s");
using var collector = new SeriesCollector(meterName);

for (var i = 0; i < 500; i++)
{
    var route = i % 2 == 0 ? "/search" : "/suggest";

    var tags = new TagList
    {
        { "http.route", route },
        { "http.response.status_code", 200 }
    };

    requestDuration.Record(0.005 + (i % 10) / 1000.0, tags);
}

Console.WriteLine($"Measurements={collector.MeasurementCount}");
Console.WriteLine($"DistinctSeries={collector.DistinctSeries}");

var valid = collector.MeasurementCount == 500 && collector.DistinctSeries <= 4;
Console.WriteLine(valid ? "VERIFY_PASS" : "VERIFY_FAIL");
return valid ? 0 : 1;

sealed class SeriesCollector : IDisposable
{
    private readonly MeterListener _listener = new();
    private readonly ConcurrentDictionary<string, byte> _series = new();
    private int _measurementCount;

    public SeriesCollector(string observedMeterName)
    {
        _listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == observedMeterName)
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _listener.SetMeasurementEventCallback<double>((_, _, tags, _) =>
        {
            var parts = new List<string>(tags.Length);
            foreach (var tag in tags)
            {
                parts.Add($"{tag.Key}={tag.Value}");
            }

            parts.Sort(StringComparer.Ordinal);
            _series.TryAdd(string.Join("|", parts), 0);
            Interlocked.Increment(ref _measurementCount);
        });

        _listener.Start();
    }

    public int MeasurementCount => Volatile.Read(ref _measurementCount);
    public int DistinctSeries => _series.Count;

    public void Dispose() => _listener.Dispose();
}
