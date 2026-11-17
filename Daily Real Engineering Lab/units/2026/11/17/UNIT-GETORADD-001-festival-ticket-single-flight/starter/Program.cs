using System.Collections.Concurrent;
using System.Threading;

var gate = new ManualResetEventSlim(false);
var arrived = new CountdownEvent(2);
var allocated = 0;
var cache = new ConcurrentDictionary<string, int>();

int Allocate(string key)
{
    arrived.Signal();
    gate.Wait();
    return Interlocked.Increment(ref allocated);
}
var a = Task.Run(() => cache.GetOrAdd("FEST-2027", Allocate));
var b = Task.Run(() => cache.GetOrAdd("FEST-2027", Allocate));
if (!arrived.Wait(TimeSpan.FromSeconds(5)))
{
    gate.Set();
    throw new Exception("Không đủ hai factory calls để reproduce; kiểm tra scheduling.");
}
gate.Set();
await Task.WhenAll(a, b);
Console.WriteLine($"Stored={cache["FEST-2027"]}; Returned={a.Result},{b.Result}; Allocations={allocated}");
if (allocated != 1) Environment.ExitCode = 2;
