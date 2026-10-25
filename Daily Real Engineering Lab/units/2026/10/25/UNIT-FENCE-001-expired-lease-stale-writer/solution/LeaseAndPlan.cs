namespace FleetRoutingLab;

public sealed record LeaseToken(string Route, string Owner, long Epoch, long ExpiresAtTick);
public sealed record PlanSnapshot(string Route, string PlanId, long LeaseEpoch);

public sealed class ManualClock
{
    public long Tick { get; private set; }
    public void Advance(long ticks)
    {
        if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
        Tick = checked(Tick + ticks);
    }
}

public sealed class LeaseRegistry
{
    private readonly ManualClock _clock;
    private readonly object _gate = new();
    private readonly Dictionary<string, LeaseToken> _active = new();
    private readonly Dictionary<string, long> _epochs = new();

    public LeaseRegistry(ManualClock clock) => _clock = clock;

    public LeaseToken Acquire(string route, string owner, long ttlTicks)
    {
        if (string.IsNullOrWhiteSpace(route) || string.IsNullOrWhiteSpace(owner) || ttlTicks <= 0)
            throw new ArgumentException("A route, owner and positive TTL are required.");

        lock (_gate)
        {
            if (_active.TryGetValue(route, out var held) && held.ExpiresAtTick > _clock.Tick)
                throw new InvalidOperationException("Route is already leased.");

            long epoch = checked(_epochs.GetValueOrDefault(route) + 1);
            var token = new LeaseToken(route, owner, epoch, checked(_clock.Tick + ttlTicks));
            _epochs[route] = epoch;
            _active[route] = token;
            return token;
        }
    }
}

public sealed class PlanStore
{
    private readonly object _gate = new();
    private readonly Dictionary<string, PlanSnapshot> _plans = new();

    public bool TryCommit(LeaseToken token, string planId)
    {
        if (string.IsNullOrWhiteSpace(planId)) throw new ArgumentException("PlanId is required.");
        lock (_gate)
        {
            if (_plans.TryGetValue(token.Route, out var current)
                && token.Epoch < current.LeaseEpoch)
                return false;

            _plans[token.Route] = new PlanSnapshot(token.Route, planId, token.Epoch);
            return true;
        }
    }

    public PlanSnapshot? Read(string route)
    {
        lock (_gate)
            return _plans.GetValueOrDefault(route);
    }
}
