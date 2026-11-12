using System.Collections.Concurrent;
using System.Threading;

var mode = args.FirstOrDefault() ?? "--run";
var issuer = new ReceiptIssuer();
var cache = new ConcurrentDictionary<string, Lazy<string>>();
var key = "toll-crossing-4821";
using var start = new ManualResetEventSlim(false);
string Issue(string crossing) => issuer.Issue(crossing);
string Get(string crossing)
{
    start.Wait();
    return cache.GetOrAdd(crossing,
        k => new Lazy<string>(() => Issue(k), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
}
var a = Task.Run(() => Get(key));
var b = Task.Run(() => Get(key));
start.Set();
var results = await Task.WhenAll(a, b);
Console.WriteLine($"returned={string.Join(",", results)}");
Console.WriteLine($"issued={issuer.Count}; distinctReturned={results.Distinct().Count()}");
if (mode == "--verify")
{
    if (issuer.Count != 1 || results.Distinct().Count() != 1)
        throw new Exception("CONTRACT_FAILED");
    Console.WriteLine("CONTRACT_PASSED");
}
else if (mode != "--run") throw new ArgumentException("Use --run or --verify");
sealed class ReceiptIssuer
{
    private int _count;
    public int Count => Volatile.Read(ref _count);
    public string Issue(string crossing) => $"{crossing}-R{Interlocked.Increment(ref _count):D3}";
}
