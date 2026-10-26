var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "";
if (mode is not ("reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project starter/ShiftSyncLab.csproj -- reproduce|verify");
    return 2;
}
CrewDutyUpdate[] feed =
[
    new("F-101", 7, "FERRY-A", "evt-107"),
    new("F-202", 3, "FERRY-B", "evt-203"),
    new("F-101", 8, "FERRY-C", "evt-108b"),
    new("F-303", 1, "FERRY-D", "evt-301"),
    new("F-101", 8, "FERRY-C", "evt-108a")
];
var forward = ShiftReconciler.Reconcile(feed);
var reverse = ShiftReconciler.Reconcile(feed.Reverse().ToArray());
static CrewDutyUpdate Find(IReadOnlyList<CrewDutyUpdate> items, string id) =>
    items.Single(x => StringComparer.Ordinal.Equals(x.CrewId, id));
Console.WriteLine("forward.F-101=" + Find(forward, "F-101"));
Console.WriteLine("reverse.F-101=" + Find(reverse, "F-101"));
Console.WriteLine("forward.count=" + forward.Count + " reverse.count=" + reverse.Count);
if (mode == "reproduce")
{
    var reproduced = Find(forward, "F-101").Revision == 7
        && Find(reverse, "F-101").Revision == 8
        && forward.Count == 3 && reverse.Count == 3;
    Console.WriteLine(reproduced ? "REPRODUCED: same feed, different revision" : "NOT_REPRODUCED");
    return reproduced ? 0 : 1;
}
var expected = new[]
{
    new CrewDutyUpdate("F-101", 8, "FERRY-C", "evt-108a"),
    new CrewDutyUpdate("F-202", 3, "FERRY-B", "evt-203"),
    new CrewDutyUpdate("F-303", 1, "FERRY-D", "evt-301")
};
var stable = forward.SequenceEqual(expected) && reverse.SequenceEqual(expected);
CrewDutyUpdate[] replayFeed =
[
    new("F-404", 4, "FERRY-P", "evt-404-old"),
    new("F-404", 5, "FERRY-Q", "evt-404-new"),
    new("F-404", 5, "FERRY-Q", "evt-404-replay")
];
var replayA = ShiftReconciler.Reconcile(replayFeed);
var replayB = ShiftReconciler.Reconcile(replayFeed.Reverse().ToArray());
var replayValid = replayA.Count == 1 && replayB.Count == 1
    && Find(replayA, "F-404").Revision == 5 && Find(replayB, "F-404").Revision == 5
    && Find(replayA, "F-404").EventId == "evt-404-new"
    && Find(replayB, "F-404").EventId == "evt-404-new";
var conflictRejected = false;
try
{
    _ = ShiftReconciler.Reconcile(
    [
        new("F-505", 9, "FERRY-R", "evt-505a"),
        new("F-505", 9, "FERRY-S", "evt-505b")
    ]);
}
catch (InvalidOperationException)
{
    conflictRejected = true;
}
Console.WriteLine("stable=" + stable + " replay=" + replayValid + " conflict=" + conflictRejected);
var valid = stable && replayValid && conflictRejected;
Console.WriteLine(valid ? "PASS: reconciliation contract" : "FAIL: reconciliation contract");
return valid ? 0 : 1;
