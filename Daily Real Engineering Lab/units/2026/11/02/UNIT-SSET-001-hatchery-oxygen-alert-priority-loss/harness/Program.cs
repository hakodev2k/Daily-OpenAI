using HatcheryLab;
var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "inspect";
if (mode is not ("inspect" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: inspect | reproduce | verify");
    return 2;
}
var at = new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);
var incoming = new[]
{
    new OxygenAlert("OX-101", 5, at, "TANK-A"),
    new OxygenAlert("OX-102", 5, at, "TANK-B"),
    new OxygenAlert("OX-103", 5, at.AddMinutes(1), "TANK-B"),
    new OxygenAlert("OX-201", 3, at.AddMinutes(-1), "TANK-A"),
    new OxygenAlert("OX-301", 1, at.AddMinutes(3), "TANK-C")
};
var queue = new AlertQueue();
var accepted = incoming.Select(queue.Enqueue).ToArray();
var snapshot = queue.Snapshot();
var actualIds = snapshot.Select(x => x.AlertId).ToArray();
string[] expectedIds = ["OX-101", "OX-102", "OX-103", "OX-201", "OX-301"];
if (mode == "inspect")
{
    Console.WriteLine($"Incoming={incoming.Length}; accepted={accepted.Count(x => x)}; retained={snapshot.Count}");
    Console.WriteLine("Snapshot=" + string.Join(", ", snapshot.Select(x => $"{x.AlertId}/{x.TankId}/S{x.Severity}")));
    return 0;
}
if (mode == "reproduce")
{
    bool reproduced = incoming.Length == 5 && snapshot.Count == 3 &&
        accepted.Count(x => x) == 3 && !actualIds.Contains("OX-102") && !actualIds.Contains("OX-103");
    Console.WriteLine(reproduced
        ? "REPRODUCED: 5 incoming alerts, only 3 retained; two independent critical events missing."
        : "NOT REPRODUCED: expected starter symptom not observed; check whether starter was edited.");
    return reproduced ? 0 : 1;
}
var results = new (string Name, bool Pass)[]
{
    ("Retain all five independent alerts", snapshot.Count == 5 && accepted.All(x => x)),
    ("Priority, timestamp and ID order", actualIds.SequenceEqual(expectedIds)),
    ("Both critical events from TANK-B survive", snapshot.Count(x => x.TankId == "TANK-B" && x.Severity == 5) == 2),
    ("Exact duplicate event is not added", !queue.Enqueue(incoming[0]) && queue.Snapshot().Count == 5)
};
foreach (var result in results)
    Console.WriteLine($"{(result.Pass ? "PASS" : "FAIL")}: {result.Name}");
Console.WriteLine($"TOTAL: {results.Count(x => x.Pass)}/{results.Length}");
return results.All(x => x.Pass) ? 0 : 1;
