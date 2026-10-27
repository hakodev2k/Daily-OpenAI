using System;
using System.Collections.Generic;
using System.Linq;

var scenarios = new[]
{
    new Scenario(
        "ordinary",
        new[]
        {
            new SettlementAdjustment("ORD-03", 25),
            new SettlementAdjustment("ORD-01", -10),
            new SettlementAdjustment("ORD-02", 0)
        },
        new[] { "ORD-01", "ORD-02", "ORD-03" }),
    new Scenario(
        "boundary",
        new[]
        {
            new SettlementAdjustment("NEG", -2_000_000_000),
            new SettlementAdjustment("POS", 2_000_000_000)
        },
        new[] { "NEG", "POS" }),
    new Scenario(
        "tie",
        new[]
        {
            new SettlementAdjustment("Z-9", 50),
            new SettlementAdjustment("A-1", 50)
        },
        new[] { "A-1", "Z-9" })
};

var selected = args.Length == 0 ? "all" :
    args.Length == 2 && args[0] == "--scenario" ? args[1] : "";
if (selected != "all" && !scenarios.Any(s => s.Name == selected))
{
    Console.Error.WriteLine("Usage: --scenario ordinary|boundary|tie|all");
    return 64;
}

var failures = 0;
foreach (var scenario in scenarios.Where(s => selected == "all" || s.Name == selected))
{
    var rows = scenario.Rows.ToArray();
    Array.Sort(rows, SettlementAdjustmentComparer.Instance);
    var actual = rows.Select(x => x.Reference).ToArray();
    var passed = actual.SequenceEqual(scenario.ExpectedReferences, StringComparer.Ordinal);
    Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {scenario.Name}: expected={string.Join(",", scenario.ExpectedReferences)} actual={string.Join(",", actual)}");
    if (!passed) failures++;
}

Console.WriteLine($"Scenarios failed: {failures}");
return failures == 0 ? 0 : 2;

record SettlementAdjustment(string Reference, int DeltaMinorUnits);
record Scenario(string Name, SettlementAdjustment[] Rows, string[] ExpectedReferences);

sealed class SettlementAdjustmentComparer : IComparer<SettlementAdjustment>
{
    public static readonly SettlementAdjustmentComparer Instance = new();

    public int Compare(SettlementAdjustment? left, SettlementAdjustment? right)
    {
        if (ReferenceEquals(left, right)) return 0;
        if (left is null) return -1;
        if (right is null) return 1;
        var byAmount = left.DeltaMinorUnits.CompareTo(right.DeltaMinorUnits);
        return byAmount != 0
            ? byAmount
            : StringComparer.Ordinal.Compare(left.Reference, right.Reference);
    }
}
