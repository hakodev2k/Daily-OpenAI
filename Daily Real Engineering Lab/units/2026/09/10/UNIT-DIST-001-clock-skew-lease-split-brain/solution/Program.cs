using System.Collections.Concurrent;

var centralClock = new ManualClock(new DateTimeOffset(2026, 9, 10, 4, 0, 0, TimeSpan.Zero));
var nodeAClock = new OffsetClock(centralClock, TimeSpan.Zero);
var nodeBClock = new OffsetClock(centralClock, TimeSpan.FromSeconds(40));
var store = new LeaseStore(centralClock);
var ttl = TimeSpan.FromSeconds(30);

var aAcquired = store.TryAcquire("invoice-2026-09-10", "worker-a", nodeAClock.UtcNow, ttl);
centralClock.Advance(TimeSpan.FromSeconds(5));
var bEarlyAcquired = store.TryAcquire("invoice-2026-09-10", "worker-b", nodeBClock.UtcNow, ttl);
var doubleProcessing = aAcquired && bEarlyAcquired;

centralClock.Set(new DateTimeOffset(2026, 9, 10, 4, 0, 31, TimeSpan.Zero));
var bAfterExpiryAcquired = store.TryAcquire("invoice-2026-09-10", "worker-b", nodeBClock.UtcNow, ttl);

Console.WriteLine($"A_ACQUIRED={aAcquired}");
Console.WriteLine($"B_EARLY_ACQUIRED={bEarlyAcquired}");
Console.WriteLine($"DOUBLE_PROCESSING={doubleProcessing}");
Console.WriteLine($"B_AFTER_EXPIRY_ACQUIRED={bAfterExpiryAcquired}");
Console.WriteLine(aAcquired && !bEarlyAcquired && !doubleProcessing && bAfterExpiryAcquired ? "REFERENCE_PASS" : "REFERENCE_FAIL");

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class ManualClock : IClock
{
    public ManualClock(DateTimeOffset initialUtc) => UtcNow = initialUtc;
    public DateTimeOffset UtcNow { get; private set; }
    public void Advance(TimeSpan delta) => UtcNow = UtcNow.Add(delta);
    public void Set(DateTimeOffset value) => UtcNow = value;
}

public sealed class OffsetClock : IClock
{
    private readonly IClock _inner;
    private readonly TimeSpan _offset;
    public OffsetClock(IClock inner, TimeSpan offset) { _inner = inner; _offset = offset; }
    public DateTimeOffset UtcNow => _inner.UtcNow.Add(_offset);
}

public sealed class LeaseStore
{
    private readonly ConcurrentDictionary<string, LeaseRecord> _leases = new();
    private readonly IClock _storageClock;

    public LeaseStore(IClock storageClock) => _storageClock = storageClock;

    public bool TryAcquire(string partition, string owner, DateTimeOffset workerObservedNow, TimeSpan ttl)
    {
        _ = workerObservedNow; // caller time is deliberately not authoritative.

        while (true)
        {
            var now = _storageClock.UtcNow;

            if (!_leases.TryGetValue(partition, out var current))
            {
                if (_leases.TryAdd(partition, new LeaseRecord(owner, now.Add(ttl))))
                {
                    return true;
                }

                continue;
            }

            if (current.ExpiresAtUtc > now)
            {
                return current.Owner == owner;
            }

            if (_leases.TryUpdate(partition, new LeaseRecord(owner, now.Add(ttl)), current))
            {
                return true;
            }
        }
    }

    private sealed record LeaseRecord(string Owner, DateTimeOffset ExpiresAtUtc);
}
