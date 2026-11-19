using System.Collections.Generic;
using System.Linq;

var metrics = new ClinicMetrics();
var visits = Enumerable.Range(1, 240).Select(i =>
    new Visit($"visit-{i:D4}", i % 3 == 0 ? "/api/appointments" : "/api/patients", i % 7 == 0 ? 503 : 200));
foreach (var visit in visits) metrics.Record(visit);
Console.WriteLine($"requests={metrics.Total} series={metrics.SeriesCount} statuses={string.Join(",", metrics.Statuses.Order())}");
if (args.Contains("--verify"))
{
    if (metrics.Total != 240 || metrics.SeriesCount > 4 || !metrics.Statuses.SetEquals(new[] { 200, 503 }))
    {
        Console.Error.WriteLine("FAIL: telemetry contract (requests=240, series<=4, statuses=200,503)");
        return 1;
    }
    Console.WriteLine("PASS: bounded metric series and preserved status classes");
    return 0;
}
if (args.Contains("--reproduce"))
{
    if (metrics.SeriesCount <= 4) { Console.Error.WriteLine("FAIL: expected baseline symptom not observed"); return 1; }
    Console.WriteLine("REPRODUCED: baseline creates more than four series");
    return 0;
}
return 0;

public sealed record Visit(string VisitId, string Route, int StatusCode);

public sealed class ClinicMetrics
{
    private readonly Dictionary<string, int> _series = new(StringComparer.Ordinal);
    private readonly HashSet<int> _statuses = new();
    public int Total { get; private set; }
    public int SeriesCount => _series.Count;
    public HashSet<int> Statuses => _statuses;
    public void Record(Visit visit)
    {
        var seriesKey = $"{visit.Route}|{visit.StatusCode}";
        _series[seriesKey] = _series.GetValueOrDefault(seriesKey) + 1;
        _statuses.Add(visit.StatusCode);
        Total++;
    }
}
