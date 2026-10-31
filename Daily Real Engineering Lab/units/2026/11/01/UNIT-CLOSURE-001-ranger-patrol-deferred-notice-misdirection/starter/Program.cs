using PatrolLab;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]");
    return 64;
}

bool baseline = args[0] == "reproduce";
PatrolNotice[] expected =
[
    new("R-201", "ranger-ana", "North ridge handoff"),
    new("R-202", "ranger-binh", "River bridge handoff"),
    new("R-203", "ranger-chi", "South trail handoff"),
    new("R-204", "ranger-dung", "Lookout tower handoff")
];

var queue = new DeferredQueue();
var gateway = new FakeGateway();

if (baseline)
{
    new BaselineScheduler().Schedule(expected, queue, gateway);
}
else
{
    new NotificationBatchScheduler().Schedule(expected, queue, gateway);
}

bool pendingCorrect = queue.PendingCount == expected.Length;
bool noEarlySend = gateway.Sent.Count == 0;
queue.Drain();

Console.WriteLine($"mode={args[0]} pendingCorrect={pendingCorrect} noEarlySend={noEarlySend}");
for (int i = 0; i < gateway.Sent.Count; i++)
{
    var notice = gateway.Sent[i];
    Console.WriteLine($"delivered[{i}]={notice.PatrolId}/{notice.RecipientId}");
}

bool exact = gateway.Sent.SequenceEqual(expected);
bool lastRecipientRepeated = gateway.Sent.Count == expected.Length &&
    gateway.Sent.All(n => n == expected[^1]);

if (baseline)
{
    bool symptomObserved = pendingCorrect && noEarlySend && !exact && lastRecipientRepeated;
    Console.WriteLine(symptomObserved
        ? "REPRODUCED: four jobs queued, deliveries diverge from requested recipients."
        : "REPRODUCTION FAILED: expected baseline symptom was not observed.");
    return symptomObserved ? 0 : 1;
}

bool verified = pendingCorrect && noEarlySend && exact;
Console.WriteLine(verified
    ? "VERIFY PASS: exactly one correct deferred delivery per notice, in order."
    : "VERIFY FAIL: incorrect recipient, order, count, or enqueue timing.");
return verified ? 0 : 1;
