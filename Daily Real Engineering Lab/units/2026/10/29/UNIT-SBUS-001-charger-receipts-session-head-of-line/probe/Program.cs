using Lab;

// Local deterministic model: one head-of-queue attempt per session per cycle;
// distinct sessions may advance in the same cycle. This is NOT an Azure emulator.
if (args.Length != 1 || !new[] { "inspect", "reproduce", "verify" }.Contains(args[0]))
{
    Console.Error.WriteLine("Usage: dotnet run --project probe -- inspect|reproduce|verify");
    return 2;
}

DispatchMessage[] input =
[
    new("A-poison", "operator-17", "charger-A", true),
    new("B-ok",     "operator-17", "charger-B", false),
    new("A-next",   "operator-17", "charger-A", false),
    new("C-ok",     "operator-88", "charger-C", false),
    new("D-ok",     "operator-88", "charger-B", false)
];
var report = Simulate(input, cycles: 5, maxDeliveryCount: 4);
foreach (var line in report.Trace) Console.WriteLine(line);
Console.WriteLine("DELIVERED " + string.Join(", ", report.Delivered.Select(x => x.Id + "@" + x.Cycle)));
Console.WriteLine("DLQ " + string.Join(", ", report.DeadLettered.Select(x => x.Id + "@" + x.Cycle)));
Console.WriteLine("PENDING " + string.Join(", ", report.Pending));
if (args[0] == "inspect") return 0;

int? Cycle(string id) => report.Delivered.FirstOrDefault(x => x.Id == id)?.Cycle;
int? DeadLetterCycle(string id) => report.DeadLettered.FirstOrDefault(x => x.Id == id)?.Cycle;
var expected = args[0] == "reproduce"
    ? new List<(string Name, bool Ok)>
    {
        ("baseline B-ok is delayed until cycle 5", Cycle("B-ok") == 5),
        ("baseline A-next remains pending at cycle 5", Cycle("A-next") is null && report.Pending.Contains("A-next")),
        ("unrelated operator C-ok succeeds at cycle 1", Cycle("C-ok") == 1),
        ("poison message reaches DLQ at cycle 4", DeadLetterCycle("A-poison") == 4)
    }
    : new List<(string Name, bool Ok)>
    {
        ("independent charger B-ok completes in cycle 1", Cycle("B-ok") == 1),
        ("same-charger A-next stays ordered after poison DLQ", Cycle("A-next") == 5),
        ("other operator C-ok completes in cycle 1", Cycle("C-ok") == 1),
        ("other operator D-ok completes in cycle 1", Cycle("D-ok") == 1),
        ("poison message reaches DLQ at cycle 4", DeadLetterCycle("A-poison") == 4),
        ("same charger shares a session", SessionPartitioner.ResolveSessionId(input[0]) == SessionPartitioner.ResolveSessionId(input[2])),
        ("different chargers of same operator use different sessions", SessionPartitioner.ResolveSessionId(input[0]) != SessionPartitioner.ResolveSessionId(input[1])),
        ("same charger across operators uses different sessions", SessionPartitioner.ResolveSessionId(input[1]) != SessionPartitioner.ResolveSessionId(input[4])),
        ("no receipt is lost or delivered twice", report.Delivered.Select(x => x.Id).Concat(report.DeadLettered.Select(x => x.Id)).Concat(report.Pending).OrderBy(x => x).SequenceEqual(input.Select(x => x.Id).OrderBy(x => x)))
    };
foreach (var check in expected) Console.WriteLine((check.Ok ? "PASS " : "FAIL ") + check.Name);
return expected.All(x => x.Ok) ? 0 : 1;

static SimulationReport Simulate(IReadOnlyList<DispatchMessage> input, int cycles, int maxDeliveryCount)
{
    var queues = input.GroupBy(SessionPartitioner.ResolveSessionId)
        .ToDictionary(x => x.Key, x => new Queue<DispatchMessage>(x));
    var attempts = new Dictionary<string, int>(StringComparer.Ordinal);
    var delivered = new List<Outcome>();
    var deadLettered = new List<Outcome>();
    var trace = new List<string>();
    foreach (var (session, queue) in queues.OrderBy(x => x.Key, StringComparer.Ordinal))
        trace.Add("SESSION " + session + " => " + string.Join(",", queue.Select(x => x.Id)));

    for (var cycle = 1; cycle <= cycles; cycle++)
    {
        foreach (var (session, queue) in queues.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (queue.Count == 0) continue;
            var current = queue.Peek();
            if (current.IsPoison)
            {
                var attempt = attempts.GetValueOrDefault(current.Id) + 1;
                attempts[current.Id] = attempt;
                trace.Add($"cycle={cycle} session={session} reject={current.Id} attempt={attempt}");
                if (attempt >= maxDeliveryCount)
                {
                    queue.Dequeue();
                    deadLettered.Add(new(current.Id, cycle));
                    trace.Add($"cycle={cycle} session={session} DLQ={current.Id}");
                }
            }
            else
            {
                queue.Dequeue();
                delivered.Add(new(current.Id, cycle));
                trace.Add($"cycle={cycle} session={session} accepted={current.Id}");
            }
        }
    }
    return new(delivered, deadLettered, queues.Values.SelectMany(x => x.Select(m => m.Id)).ToList(), trace);
}

internal sealed record Outcome(string Id, int Cycle);
internal sealed record SimulationReport(
    IReadOnlyList<Outcome> Delivered,
    IReadOnlyList<Outcome> DeadLettered,
    IReadOnlyList<string> Pending,
    IReadOnlyList<string> Trace);
