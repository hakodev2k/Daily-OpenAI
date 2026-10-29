using WeighbridgeLab;

var mode = args.Length == 0 ? "report" : args[0].ToLowerInvariant();
var builder = new ReportBuilder();

static RotatingReceiptFeed NewLiveFeed() => new(
    new Receipt[] { new("G-101", 100m), new("G-102", 200m) },
    new Receipt[] { new("G-101", 100m), new("G-102", 200m), new("G-103", 400m) });

var live = NewLiveFeed();
var report = builder.Build(live);
Console.WriteLine($"ReceiptCount={report.ReceiptCount}; TotalWeightKg={report.TotalWeightKg}; AverageWeightKg={report.AverageWeightKg}; SnapshotOpens={live.SnapshotOpens}");

if (mode == "report") return 0;

if (mode == "reproduce")
{
    bool symptom = report.ReceiptCount == 2 && report.TotalWeightKg == 700m && live.SnapshotOpens == 2;
    Console.WriteLine(symptom ? "REPRODUCED: inconsistent live report" : "NOT REPRODUCED: starter symptom absent");
    return symptom ? 0 : 1;
}

if (mode == "verify")
{
    bool liveOk = report.ReceiptCount == 2 && report.TotalWeightKg == 300m && live.SnapshotOpens == 1;
    var empty = new RotatingReceiptFeed(Array.Empty<Receipt>());
    var emptyReport = builder.Build(empty);
    bool emptyOk = emptyReport.ReceiptCount == 0 && emptyReport.TotalWeightKg == 0m &&
                   emptyReport.AverageWeightKg == 0m && empty.SnapshotOpens == 1;

    var stable = new RotatingReceiptFeed(new Receipt[]
    {
        new("G-201", 1.25m), new("G-202", 2.75m)
    });
    var stableReport = builder.Build(stable);
    bool stableOk = stableReport.ReceiptCount == 2 && stableReport.TotalWeightKg == 4m &&
                    stableReport.AverageWeightKg == 2m && stable.SnapshotOpens == 1;

    Console.WriteLine($"LiveConsistency={(liveOk ? "PASS" : "FAIL")}; Empty={(emptyOk ? "PASS" : "FAIL")}; Stable={(stableOk ? "PASS" : "FAIL")}");
    return liveOk && emptyOk && stableOk ? 0 : 1;
}

Console.Error.WriteLine("Usage: report | reproduce | verify");
return 2;
