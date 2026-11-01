using System;
using System.Linq;

var cases = new[]
{
    new PolicyCase("three-ready-80pct", 3, 3, 80, 0),
    new PolicyCase("five-ready-80pct", 5, 5, 80, 1),
    new PolicyCase("four-ready-75pct", 4, 4, 75, 1),
    new PolicyCase("four-one-unready-75pct", 4, 3, 75, 0),
    new PolicyCase("two-ready-51pct", 2, 2, 51, 0),
    new PolicyCase("six-ready-50pct", 6, 6, 50, 3),
    new PolicyCase("zero-ready-80pct", 3, 0, 80, 0)
};
var outcomes = cases.Select(c => new Outcome(c, PdbCalculator.AllowedDisruptions(
    c.ExpectedPods, c.HealthyPods, c.MinAvailablePercent))).ToArray();
foreach (var x in outcomes)
    Console.WriteLine($"{x.Input.Name}: expected={x.Input.ExpectedAllowed}, model={x.Actual}, pass={x.Pass}");
int mismatches = outcomes.Count(x => !x.Pass);
Console.WriteLine($"CONTRACT_MISMATCHES={mismatches}");
var mode = args.FirstOrDefault() ?? "inspect";
return mode switch
{
    "reproduce" => mismatches >= 2 && outcomes.Single(x => x.Input.Name == "five-ready-80pct").Pass ? 0 : 2,
    "verify" => mismatches == 0 ? 0 : 3,
    "inspect" => 0,
    _ => 4
};

internal sealed record PolicyCase(string Name, int ExpectedPods, int HealthyPods, int MinAvailablePercent, int ExpectedAllowed);
internal sealed record Outcome(PolicyCase Input, int Actual)
{
    public bool Pass => Actual == Input.ExpectedAllowed;
}
