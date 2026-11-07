using FerryDispatchLab;
var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
var jobs = new[]
{
    new DispatchJob("F-204", 5, 10),
    new DispatchJob("F-205", 5, 11),
    new DispatchJob("F-206", 3, 12),
    new DispatchJob("F-207", 5, 9),
    new DispatchJob("F-208", 5, 11)
};
var queue = new DispatchQueue();
var accepted = jobs.Select(queue.Enqueue).ToArray();
var snapshot = queue.Snapshot();
Console.WriteLine("Received=" + jobs.Length + "; Accepted=" + accepted.Count(x => x)
    + "; Pending=" + snapshot.Count);
Console.WriteLine("Order=" + string.Join(",", snapshot.Select(x => x.JobId)));
if (mode == "run") return 0;
if (mode == "reproduce")
{
    bool symptom = accepted.Count(x => x) == 2 && snapshot.Count == 2;
    Console.WriteLine(symptom ? "REPRODUCED: missing dispatch jobs" : "NOT REPRODUCED");
    return symptom ? 0 : 1;
}
if (mode == "verify")
{
    bool noLoss = accepted.All(x => x) && snapshot.Count == jobs.Length;
    var expected = new[] { "F-207", "F-204", "F-205", "F-208", "F-206" };
    bool ordered = snapshot.Select(x => x.JobId).SequenceEqual(expected);
    bool duplicateRejected = !queue.Enqueue(jobs[0]);
    Console.WriteLine("NoLoss=" + (noLoss ? "PASS" : "FAIL")
        + "; StableOrder=" + (ordered ? "PASS" : "FAIL")
        + "; ExactDuplicate=" + (duplicateRejected ? "PASS" : "FAIL"));
    return noLoss && ordered && duplicateRejected ? 0 : 1;
}
Console.Error.WriteLine("Usage: run | reproduce | verify");
return 2;
