using FieldSurvey;

static SurveyReport MakeReport(string condition = "HEALTHY") => new()
{
    SiteId = "MANGROVE-SECTOR-7",
    Readings = new List<SurveyReading>
    {
        new()
        {
            PlotId = "PLOT-A",
            Condition = condition,
            Notes = new List<string> { "initial field observation" }
        }
    }
};

static Dictionary<string, bool> CheckContracts()
{
    var results = new Dictionary<string, bool>();

    var source = MakeReport();
    var store = new SnapshotStore();
    store.Publish(source);
    source.Readings[0].Condition = "DAMAGED";
    source.Readings[0].Notes.Add("later technician update");
    var first = store.GetHistory()[0];
    results["ingress-isolation"] =
        first.Readings[0].Condition == "HEALTHY" &&
        first.Readings[0].Notes.SequenceEqual(new[] { "initial field observation" });

    var independent = new SnapshotStore();
    independent.Publish(MakeReport());
    var exported = independent.GetHistory()[0];
    exported.Readings[0].Condition = "TAMPERED";
    exported.Readings[0].Notes.Add("reader edit");
    var reloaded = independent.GetHistory()[0];
    results["egress-isolation"] =
        reloaded.Readings[0].Condition == "HEALTHY" &&
        reloaded.Readings[0].Notes.SequenceEqual(new[] { "initial field observation" });

    store.Publish(source);
    var revisions = store.GetHistory();
    results["revision-independence"] =
        revisions.Count == 2 &&
        revisions[0].Readings[0].Condition == "HEALTHY" &&
        revisions[1].Readings[0].Condition == "DAMAGED" &&
        revisions[0].Readings[0].PlotId == "PLOT-A";

    var listStore = new SnapshotStore();
    listStore.Publish(MakeReport());
    var returned = listStore.GetHistory();
    returned[0].Readings.Clear();
    results["nested-list-isolation"] =
        listStore.GetHistory().Count == 1 &&
        listStore.GetHistory()[0].Readings.Count == 1;

    return results;
}

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "inspect";
if (mode is not ("reproduce" or "verify" or "inspect"))
{
    Console.Error.WriteLine("Usage: dotnet run --project harness/Contracts.csproj -- reproduce|verify|inspect");
    return 2;
}
var checks = CheckContracts();
foreach (var check in checks)
    Console.WriteLine($"{(check.Value ? "PASS" : "FAIL")} {check.Key}");
if (mode == "inspect") return 0;
if (mode == "reproduce")
{
    var reproduced = checks.Count == 4 && checks.Values.All(v => !v);
    Console.WriteLine(reproduced ? "EXPECTED FAILURE REPRODUCED" : "REPRODUCTION CONTRACT DID NOT MATCH STARTER");
    return reproduced ? 0 : 1;
}
var verified = checks.Count == 4 && checks.Values.All(v => v);
Console.WriteLine(verified ? "ALL CONTRACTS PASS" : "VERIFICATION FAILED: learner-editable starter still violates snapshot isolation");
return verified ? 0 : 1;
