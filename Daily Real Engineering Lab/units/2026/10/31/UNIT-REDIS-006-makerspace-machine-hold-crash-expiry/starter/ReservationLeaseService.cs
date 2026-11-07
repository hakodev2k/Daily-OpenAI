namespace Lab;

public sealed class ReservationLeaseService
{
    private readonly SimulatedRedis _redis;
    private readonly CrashInjector _crash;

    public ReservationLeaseService(SimulatedRedis redis, CrashInjector crash)
    {
        _redis = redis;
        _crash = crash;
    }

    public bool TryReserve(string machineSlot, string requestId, TimeSpan ttl)
    {
        if (string.IsNullOrWhiteSpace(machineSlot))
            throw new ArgumentException("Machine slot is required.", nameof(machineSlot));
        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("Request ID is required.", nameof(requestId));
        if (ttl <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(ttl));

        string key = $"hold:{machineSlot}";
        if (!_redis.SetIfAbsent(key, requestId))
            return false;

        _crash.AfterPersist();
        _redis.Expire(key, ttl);
        return true;
    }
}
