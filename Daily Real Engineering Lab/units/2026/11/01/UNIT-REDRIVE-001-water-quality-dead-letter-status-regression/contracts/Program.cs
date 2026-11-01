using ReplayLab;

var failures = new List<string>();
void Check(string name, bool valid)
{
    Console.WriteLine($"{(valid ? "PASS" : "FAIL")} {name}");
    if (!valid) failures.Add(name);
}
var p = new StatusProjection();
var v1 = new StatusSnapshot("evt-v1", "W-17", 1, "OPEN");
var v3 = new StatusSnapshot("evt-v3", "W-17", 3, "CLOSED");
var v2 = new StatusSnapshot("evt-v2", "W-17", 2, "SUSPENDED");
Check("initial snapshot", p.Apply(v1) == ApplyOutcome.Applied && p.Get("W-17")?.Status == "OPEN");
Check("newer snapshot", p.Apply(v3) == ApplyOutcome.Applied && p.Get("W-17")?.Version == 3);
var late = p.Apply(v2);
Check("late replay preserves newer status",
    late == ApplyOutcome.IgnoredStale && p.Get("W-17")?.Version == 3 && p.Get("W-17")?.Status == "CLOSED");
var duplicate = p.Apply(v3);
Check("duplicate has no additional effect",
    duplicate == ApplyOutcome.IgnoredDuplicate && p.AppliedCount == 2);
var conflict = p.Apply(new StatusSnapshot("evt-conflict", "W-17", 3, "OPEN"));
Check("same version conflicting event rejected",
    conflict == ApplyOutcome.RejectedConflict && p.Get("W-17")?.Status == "CLOSED");
var reusedId = p.Apply(new StatusSnapshot("evt-v3", "W-17", 4, "OPEN"));
Check("reused event id with changed payload rejected",
    reusedId == ApplyOutcome.RejectedConflict && p.Get("W-17")?.Version == 3);
Check("independent site accepted",
    p.Apply(new StatusSnapshot("evt-other", "W-18", 1, "OPEN")) == ApplyOutcome.Applied
    && p.Get("W-18")?.Version == 1 && p.Get("W-17")?.Status == "CLOSED");
Check("accepted change count", p.AppliedCount == 3);

if (args.Contains("--expect-broken", StringComparer.Ordinal))
{
    var required = new[]
    {
        "late replay preserves newer status",
        "same version conflicting event rejected",
        "reused event id with changed payload rejected"
    };
    var reproduced = required.All(failures.Contains);
    Console.WriteLine(reproduced ? "REPRODUCED expected starter violations" : "NOT REPRODUCED expected starter violations");
    return reproduced ? 0 : 1;
}
Console.WriteLine(failures.Count == 0 ? "VERIFIED all contracts" : $"NOT VERIFIED: {failures.Count} failed contracts");
return failures.Count == 0 ? 0 : 1;
