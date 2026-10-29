using System.Collections.Concurrent;
using System.Diagnostics;
using TraceWindowLab;

var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "run";
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Use: run | reproduce | verify");
    return 3;
}
var timeline = new ConcurrentQueue<TracePoint>();
using var listener = new ActivityListener
{
    ShouldListenTo = source => source.Name == "TraceWindowLab.Courier",
    Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
    SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllDataAndRecorded,
    ActivityStarted = a => timeline.Enqueue(TracePoint.From("START", a)),
    ActivityStopped = a => timeline.Enqueue(TracePoint.From("STOP", a))
};
ActivitySource.AddActivityListener(listener);
var gateway = new ControlledGateway();
var dispatcher = new DispatchService(gateway);
var pending = dispatcher.DispatchAsync("SPEC-42");
gateway.AllowStart();
await gateway.Entered.WaitAsync(TimeSpan.FromSeconds(5));
gateway.AllowFinish();
var receipt = await pending.WaitAsync(TimeSpan.FromSeconds(5));

var events = timeline.ToArray();
foreach (var e in events)
    Console.WriteLine($"{e.Phase,-5} {e.Operation,-12} trace={e.TraceId} span={e.SpanId} parent={e.ParentSpanId}");
int Find(string phase, string name) =>
    Array.FindIndex(events, e => e.Phase == phase && e.Operation == name);
var ps = Find("START", "dispatch");
var pe = Find("STOP", "dispatch");
var cs = Find("START", "gateway-send");
var ce = Find("STOP", "gateway-send");
bool complete = events.Length == 4 && ps >= 0 && pe >= 0 && cs >= 0 && ce >= 0;
bool related = complete && events[ps].TraceId == events[cs].TraceId &&
    events[cs].ParentSpanId == events[ps].SpanId;
bool businessOk = receipt.SpecimenId == "SPEC-42" && receipt.Status == "accepted";
bool correctOrder = complete && ps < cs && cs < ce && ce < pe;
bool originalSymptom = complete && ps < pe && pe < cs && cs < ce;
Console.WriteLine($"Business={businessOk}; TraceRelation={related}; CorrectLifetime={correctOrder}; OriginalSymptom={originalSymptom}");
if (mode == "run") return 0;
if (mode == "reproduce")
{
    if (businessOk && related && originalSymptom)
    {
        Console.WriteLine("REPRODUCED: parent span stopped before gateway began.");
        return 0;
    }
    Console.Error.WriteLine("REPRODUCTION FAILED: expected baseline symptom absent.");
    return 2;
}
if (mode == "verify")
{
    if (businessOk && related && correctOrder)
    {
        Console.WriteLine("VERIFIED: gateway completes inside dispatch span.");
        return 0;
    }
    Console.Error.WriteLine("VERIFY FAILED: inspect timeline and parent-child relationship.");
    return 1;
}
return 3;

internal sealed record TracePoint(
    string Phase, string Operation, string TraceId, string SpanId, string ParentSpanId)
{
    public static TracePoint From(string phase, Activity a) =>
        new(phase, a.OperationName, a.TraceId.ToString(),
            a.SpanId.ToString(), a.ParentSpanId.ToString());
}
