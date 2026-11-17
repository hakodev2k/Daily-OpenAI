using System.Collections.Concurrent;
using System.Threading;

var gate = new ManualResetEventSlim(false);
var arrived = new CountdownEvent(2);
var allocated = 0;
var cache = new ConcurrentDictionary<string, Lazy<int>>();

int Allocate(string key) => Interlocked.Increment(ref allocated);
var a = Task.Run(() => cache.GetOrAdd("FEST-2027", _ => new Lazy<int>(() => Allocate("FEST-2027"), LazyThreadSafetyMode.ExecutionAndPublication)).Value);
var b = Task.Run(() => cache.GetOrAdd("FEST-2027", _ => new Lazy<int>(() => Allocate("FEST-2027"), LazyThreadSafetyMode.ExecutionAndPublication)).Value);
await Task.WhenAll(a, b);
Console.WriteLine($"Stored={cache["FEST-2027"].Value}; Returned={a.Result},{b.Result}; Allocations={allocated}");
if (allocated != 1 || a.Result != b.Result) Environment.ExitCode = 2;
