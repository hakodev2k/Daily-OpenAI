using OfflineSyncLab;

static Change C(string id, string field, string value, long baseRevision, string time) =>
    new(id, field, value, baseRevision, DateTimeOffset.Parse(time, System.Globalization.CultureInfo.InvariantCulture));

var engine = new SyncEngine();
var sequence = new[]
{
    C("A-101", "Status", "InProgress", 0, "2026-10-27T10:05:00+07:00"),
    C("B-201", "TechnicianNote", "Valve seal damaged", 0, "2026-10-27T10:03:00+07:00"),
    C("B-202", "Status", "Cancelled", 0, "2026-10-27T10:07:00+07:00"),
    C("B-201", "TechnicianNote", "Valve seal damaged", 0, "2026-10-27T10:03:00+07:00"),
    C("A-102", "Status", "Completed", 1, "2026-10-27T10:01:00+07:00")
};
var results = sequence.Select(engine.Apply).ToArray();
for (int i = 0; i < sequence.Length; i++)
    Console.WriteLine($"{sequence[i].ChangeId}: {results[i]}");
Console.WriteLine($"Final: Status={engine.State.Fields["Status"]}; Note={engine.State.Fields["TechnicianNote"]}; Revision={engine.State.Revision}");

bool reproduced =
    results.SequenceEqual(new[] { ApplyStatus.Applied, ApplyStatus.Ignored, ApplyStatus.Applied, ApplyStatus.Ignored, ApplyStatus.Ignored }) &&
    engine.State.Fields["Status"] == "Cancelled" &&
    engine.State.Fields["TechnicianNote"] == "None";

bool verified =
    results.SequenceEqual(new[] { ApplyStatus.Applied, ApplyStatus.Applied, ApplyStatus.Conflict, ApplyStatus.AlreadyApplied, ApplyStatus.Applied }) &&
    engine.State.Fields["Status"] == "Completed" &&
    engine.State.Fields["TechnicianNote"] == "Valve seal damaged" &&
    engine.State.FieldRevisions["Status"] == 2 &&
    engine.State.FieldRevisions["TechnicianNote"] == 1 &&
    engine.State.Revision == 3;

if (args.Contains("--verify", StringComparer.Ordinal))
{
    var extra = new SyncEngine();
    var first = extra.Apply(C("X-1", "TechnicianNote", "Inspected", 0, "2026-10-27T11:00:00+07:00"));
    var alteredReplay = extra.Apply(C("X-1", "TechnicianNote", "Tampered", 0, "2026-10-27T11:00:00+07:00"));
    var unknown = extra.Apply(C("X-2", "Priority", "High", 0, "2026-10-27T11:01:00+07:00"));
    var stale = extra.Apply(C("X-3", "TechnicianNote", "Old", 0, "2026-10-27T10:00:00+07:00"));
    verified &= first == ApplyStatus.Applied &&
                alteredReplay == ApplyStatus.Conflict &&
                unknown == ApplyStatus.Conflict &&
                stale == ApplyStatus.Conflict &&
                extra.State.Revision == 1;
    Console.WriteLine(verified ? "VERIFIED: offline sync contract passed." : "VERIFY FAILED: expected offline sync contract not met.");
    return verified ? 0 : 1;
}
if (args.Contains("--reproduce", StringComparer.Ordinal))
{
    Console.WriteLine(reproduced ? "REPRODUCED: starter contract violation." : "REPRODUCE FAILED: starter no longer matches original failure.");
    return reproduced ? 0 : 1;
}
Console.WriteLine("Use --reproduce or --verify for assertions.");
return 0;
