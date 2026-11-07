var store = new FakeRedisLockStore();
var ttl = TimeSpan.FromSeconds(5);
var key = "customer:42";

var tokenA = "worker-a";
var tokenB = "worker-b";
var tokenC = "worker-c";

Console.WriteLine("t=0 A acquire");
Console.WriteLine($"A acquired: {store.TryAcquire(key, tokenA, ttl)}");

store.Advance(TimeSpan.FromSeconds(6));
Console.WriteLine("t=6 A lease expired; B acquire");
Console.WriteLine($"B acquired: {store.TryAcquire(key, tokenB, ttl)}");
Console.WriteLine($"owner now: {store.GetOwner(key)}");

Console.WriteLine("A finishes stale work and releases");
Release(store, key, tokenA);
Console.WriteLine($"owner after A release: {store.GetOwner(key) ?? "<none>"}");

Console.WriteLine("C tries while B is still in critical section");
var cAcquired = store.TryAcquire(key, tokenC, ttl);
Console.WriteLine($"C acquired: {cAcquired}");

if (cAcquired)
{
    Console.WriteLine("VIOLATION: another worker entered while B still believes it owns the critical section.");
    Environment.ExitCode = 2;
}
else
{
    Console.WriteLine("SAFE: current ownership was preserved.");
}

static void Release(FakeRedisLockStore store, string key, string token)
{
    // Investigation note:
    // What evidence should be required before one worker removes a shared lock key?
    store.Delete(key);
}

sealed class FakeRedisLockStore
{
    private readonly Dictionary<string, Lease> _leases = new(StringComparer.Ordinal);
    private DateTimeOffset _now = DateTimeOffset.UnixEpoch;

    public bool TryAcquire(string key, string token, TimeSpan ttl)
    {
        RemoveExpired(key);
        if (_leases.ContainsKey(key)) return false;
        _leases[key] = new Lease(token, _now + ttl);
        return true;
    }

    public void Delete(string key) => _leases.Remove(key);

    public bool DeleteIfOwner(string key, string token)
    {
        RemoveExpired(key);
        if (_leases.TryGetValue(key, out var lease) && lease.Token == token)
        {
            _leases.Remove(key);
            return true;
        }
        return false;
    }

    public string? GetOwner(string key)
    {
        RemoveExpired(key);
        return _leases.TryGetValue(key, out var lease) ? lease.Token : null;
    }

    public void Advance(TimeSpan delta) => _now += delta;

    private void RemoveExpired(string key)
    {
        if (_leases.TryGetValue(key, out var lease) && lease.ExpiresAt <= _now)
            _leases.Remove(key);
    }

    private sealed record Lease(string Token, DateTimeOffset ExpiresAt);
}
