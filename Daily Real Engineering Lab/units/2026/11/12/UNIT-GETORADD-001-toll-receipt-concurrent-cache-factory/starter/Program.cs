using System.Collections.Concurrent;
using System.Threading;

var mode = args.FirstOrDefault() ?? "--run";
var issuer = new ReceiptIssuer();
var cache = new ConcurrentDictionary<string, string>();
using var ready = new CountdownEvent(2);
using var release = new ManualResetEventSlim(false);
var key = "toll-crossing-4821";

string Issue(string crossing)
{
    var serial = issuer.Issue(crossing);
    ready.Signal();
    if (!release.Wait(TimeSpan.FromSeconds(5)))
        throw new TimeoutException("Parallel issuance gate timed out");
    return serial;
}

var a = Task.Run(() => cache.GetOrAdd(key, Issue));
var b = Task.Run(() => cache.GetOrAdd(key, Issue));
if (!ready.Wait(TimeSpan.FromSeconds(5)))
{
    release.Set();
    throw new InvalidOperationException("Could not reproduce overlapping factory calls");
}
release.Set();
var results = await Task.WhenAll(a, b);
Console.WriteLine($"returned={string.Join(",", results)}");
Console.WriteLine($"issued={issuer.Count}; distinctReturned={results.Distinct().Count()}");
if (mode == "--reproduce")
{
    if (issuer.Count != 2 || results.Distinct().Count() != 1)
        throw new Exception("Expected incident not reproduced");
    Console.WriteLine("INCIDENT_REPRODUCED");
}
else if (mode == "--verify")
{
    if (issuer.Count != 1 || results.Distinct().Count() != 1)
        throw new Exception("CONTRACT_FAILED: external issuance must happen exactly once");
    Console.WriteLine("CONTRACT_PASSED");
}
else if (mode != "--run") throw new ArgumentException("Use --run, --reproduce or --verify");

sealed class ReceiptIssuer
{
    private int _count;
    public int Count => Volatile.Read(ref _count);
    public string Issue(string crossing) => $"{crossing}-R{Interlocked.Increment(ref _count):D3}";
}
