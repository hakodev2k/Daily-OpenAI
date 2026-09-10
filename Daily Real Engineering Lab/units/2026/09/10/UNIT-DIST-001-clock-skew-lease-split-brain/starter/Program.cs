using System.Collections.Concurrent;

var verify = args.Contains("--verify", StringComparer.OrdinalIgnoreCase);

var centralClock = new ManualClock(new DateTimeOffset(2026, 9, 10, 4, 0, 0, TimeSpan.Zero));
var nodeAClock = new OffsetClock(centralClock, TimeSpan.Zero);
var nodeBClock = new OffsetClock(centralClock, TimeSpan.FromSeconds(40));
var store = new LeaseStore(centralClock);
var ttl = TimeSpan.FromSeconds(30);

var aAcquired = store.TryAcquire("invoice-2026-09-10", "worker-a", nodeAClock.UtcNow, ttl);
Console.WriteLine($"A_ACQUIRED={aAcquired}");
Console.WriteLine($"LEASE_AFTER_A={store.Describe("invoice-2026-09-10")}");

centralClock.Advance(TimeSpan.FromSeconds(5));

var bEarlyAcquired = store.TryAcquire("invoice-2026-09-10", "worker-b", nodeBClock.UtcNow, ttl);
Console.WriteLine($"B_EARLY_ACQUIRED={bEarlyAcquired}");
Console.WriteLine($"LEASE_AFTER_B_EARLY={store.Describe("invoice-2026-09-10")}");

var aStillProcessing = aAcquired && centralClock.UtcNow < new DateTimeOffset(2026, 9, 10, 4, 0, 20, TimeSpan.Zero);
var doubleProcessing = aStillProcessing && bEarlyAcquired;
Console.WriteLine($"DOUBLE_PROCESSING={doubleProcessing}");

if (!verify)
{
    return;
}

centralClock.Set(new DateTimeOffset(2026, 9, 10, 4, 0, 31, TimeSpan.Zero));
var bAfterExpiryAcquired = store.TryAcquire("invoice-2026-09-10", "worker-b", nodeBClock.UtcNow, ttl);
Console.WriteLine($"B_AFTER_EXPIRY_ACQUIRED={bAfterExpiryAcquired}");

var passed = aAcquired
             && !bEarlyAcquired
             && !doubleProcessing
             && bAfterExpiryAcquired;

Console.WriteLine(passed ? "LAB_VERIFY_PASS" : "LAB_VERIFY_FAIL");
Environment.ExitCode = passed ? 0 : 2;

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

    public OffsetClock(IClock inner, TimeSpan offset)
    {
        _inner = inner;
        _offset = offset;
    }

    public DateTimeOffset UtcNow => _inner.UtcNow.Add(_offset);
}

public sealed class LeaseStore
{
    private readonly ConcurrentDictionary<string, LeaseRecord> _leases = new();
    private readonly IClock _storageClock;

    public LeaseStore(IClock storageClock) => _storageClock = storageClock;

    public bool TryAcquire(string partition, string owner, DateTimeOffset workerObservedNow, TimeSpan ttl)
    {
        while (true)
        {
            if (!_leases.TryGetValue(partition, out var current))
            {
                var created = new LeaseRecord(owner, workerObservedNow.Add(ttl));
                if (_leases.TryAdd(partition, created))
                {
                    return true;
                }

                continue;
            }

            // Investigation note: compare the authority used for this decision
            // with the timeline shown by the simulated storage system.
            if (current.ExpiresAtUtc > workerObservedNow)
            {
                return current.Owner == owner;
            }

            var replacement = new LeaseRecord(owner, workerObservedNow.Add(ttl));
            if (_leases.TryUpdate(partition, replacement, current))
            {
                return true;
            }
        }
    }

    public string Describe(string partition)
    {
        if (!_leases.TryGetValue(partition, out var lease))
        {
            return "<none>";
        }

        return $"owner={lease.Owner}; expires={lease.ExpiresAtUtc:O}; storageNow={_storageClock.UtcNow:O}";
    }

    private sealed record LeaseRecord(string Owner, DateTimeOffset ExpiresAtUtc);
}
