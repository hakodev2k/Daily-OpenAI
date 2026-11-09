using System;
using System.Collections.Generic;
using System.Linq;

var mode = args.Length == 2 && args[0] == "--mode" ? args[1] : "run";
var source = new List<TreeObservation>
{
    new("T-101", "Oak", true),
    new("T-102", "Pine", true),
    new("T-103", "Oak", false)
};

var first = BuildReport(source, () => source.Add(new("T-104", "Maple", true)));
Console.WriteLine($"First: Count={first.Count}, Ids=[{string.Join(",", first.Ids)}], Source={source.Count}");
var second = BuildReport(source, () => { });
Console.WriteLine($"Next: Count={second.Count}, Ids=[{string.Join(",", second.Ids)}], Source={source.Count}");

var consistent = first.Count == first.Ids.Length &&
                 second.Count == second.Ids.Length &&
                 second.Count == 3 &&
                 first.Count == 2 &&
                 first.Ids.SequenceEqual(new[] { "T-101", "T-102" }) &&
                 second.Ids.SequenceEqual(new[] { "T-101", "T-102", "T-104" });

if (mode == "reproduce")
{
    if (first.Count != first.Ids.Length) { Console.WriteLine("REPRODUCED: count/list mismatch"); return 0; }
    Console.Error.WriteLine("NOT REPRODUCED: starter symptom absent"); return 1;
}
if (mode == "verify")
{
    if (consistent) { Console.WriteLine("PASS: snapshot consistency and next-report freshness"); return 0; }
    Console.Error.WriteLine("FAIL: report consistency or freshness contract"); return 1;
}
if (mode != "run") { Console.Error.WriteLine("Unknown mode"); return 2; }
return 0;

static CensusReport BuildReport(List<TreeObservation> observations, Action beforeExport)
{
    IEnumerable<TreeObservation> accepted = observations.Where(x => x.IsConfirmed);
    var count = accepted.Count();
    beforeExport(); // Field device synchronization may complete between report stages.
    var ids = accepted.Select(x => x.Id).ToArray();
    return new CensusReport(count, ids);
}

internal sealed record TreeObservation(string Id, string Species, bool IsConfirmed);
internal sealed record CensusReport(int Count, string[] Ids);
