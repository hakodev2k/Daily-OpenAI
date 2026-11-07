using System.Collections.Concurrent;
using System.Threading;

var service = new LockerReservationService();
var first = Task.Run(() => service.Reserve("PKG-204"));
var second = Task.Run(() => service.Reserve("PKG-204"));
await Task.WhenAll(first, second);
Console.WriteLine($"CALLS {service.GatewayCalls}");
Console.WriteLine($"CACHE {service.CacheCount}");
Console.WriteLine($"SAME {first.Result == second.Result}");
Console.WriteLine($"FAILURE_RETRY {service.FailureRetryWorks()}");

sealed class LockerReservationService
{
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private readonly CountdownEvent _firstTwoFactories = new(2);
    private int _gatewayCalls;
    private int _failCalls;
    public int GatewayCalls => Volatile.Read(ref _gatewayCalls);
    public int CacheCount => _cache.Count;

    public string Reserve(string parcelId) => _cache.GetOrAdd(parcelId, key =>
    {
        // Local gateway simulator: two initial calls overlap predictably.
        var n = Interlocked.Increment(ref _gatewayCalls);
        if (n <= 2)
        {
            _firstTwoFactories.Signal();
            if (!_firstTwoFactories.Wait(TimeSpan.FromSeconds(10)))
                throw new TimeoutException("Gateway synchronization timed out");
        }
        return $"locker-{n}";
    });

    public bool FailureRetryWorks()
    {
        try { ReserveFlaky("PKG-FAIL"); } catch (InvalidOperationException) { }
        return ReserveFlaky("PKG-FAIL") == "recovered" && Volatile.Read(ref _failCalls) == 2;
    }

    private readonly ConcurrentDictionary<string, string> _flakyCache = new();
    private string ReserveFlaky(string id) => _flakyCache.GetOrAdd(id, _ =>
    {
        if (Interlocked.Increment(ref _failCalls) == 1) throw new InvalidOperationException("gateway unavailable");
        return "recovered";
    });
}
