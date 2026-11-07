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
    private readonly ConcurrentDictionary<string, Lazy<string>> _cache = new();
    private int _gatewayCalls;
    private int _failCalls;
    public int GatewayCalls => Volatile.Read(ref _gatewayCalls);
    public int CacheCount => _cache.Count;

    public string Reserve(string parcelId)
    {
        var holder = _cache.GetOrAdd(parcelId, key =>
            new Lazy<string>(() => Allocate(key), LazyThreadSafetyMode.ExecutionAndPublication));
        try { return holder.Value; }
        catch
        {
            // Remove only this exact holder; do not remove a newer replacement.
            ((ICollection<KeyValuePair<string, Lazy<string>>>)_cache)
                .Remove(new KeyValuePair<string, Lazy<string>>(parcelId, holder));
            throw;
        }
    }

    private string Allocate(string id)
    {
        var n = Interlocked.Increment(ref _gatewayCalls);
        return $"locker-{n}";
    }

    public bool FailureRetryWorks()
    {
        try { ReserveFlaky("PKG-FAIL"); } catch (InvalidOperationException) { }
        return ReserveFlaky("PKG-FAIL") == "recovered" && Volatile.Read(ref _failCalls) == 2;
    }

    private readonly ConcurrentDictionary<string, Lazy<string>> _flakyCache = new();
    private string ReserveFlaky(string id)
    {
        var holder = _flakyCache.GetOrAdd(id, _ => new Lazy<string>(() =>
        {
            if (Interlocked.Increment(ref _failCalls) == 1) throw new InvalidOperationException("gateway unavailable");
            return "recovered";
        }, LazyThreadSafetyMode.ExecutionAndPublication));
        try { return holder.Value; }
        catch
        {
            ((ICollection<KeyValuePair<string, Lazy<string>>>)_flakyCache)
                .Remove(new KeyValuePair<string, Lazy<string>>(id, holder));
            throw;
        }
    }
}
