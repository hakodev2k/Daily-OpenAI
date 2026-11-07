namespace Lab;

// Mô hình tối giản của một Redis bên ngoài tiến trình API.
// Dữ liệu tồn tại khi client chết; client cũ không thể tiếp tục ghi sau crash.
public sealed class ManualClock
{
    public DateTimeOffset UtcNow { get; private set; } =
        new(2026, 10, 31, 0, 0, 0, TimeSpan.Zero);

    public void Advance(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(duration));
        UtcNow += duration;
    }
}

public sealed class SimulatedRedis
{
    private sealed record Entry(string Value, DateTimeOffset? ExpiresAt);
    private readonly Dictionary<string, Entry> _entries = new(StringComparer.Ordinal);
    private readonly ManualClock _clock;
    private bool _clientAlive = true;

    public SimulatedRedis(ManualClock clock) => _clock = clock;

    public void CrashClient() => _clientAlive = false;
    public void RestartClient() => _clientAlive = true;

    private void RequireLiveClient()
    {
        if (!_clientAlive) throw new SimulatedCrashException();
    }

    private void PurgeExpired(string key)
    {
        if (_entries.TryGetValue(key, out var current) &&
            current.ExpiresAt is { } until && until <= _clock.UtcNow)
            _entries.Remove(key);
    }

    public bool SetIfAbsent(string key, string value)
    {
        RequireLiveClient();
        PurgeExpired(key);
        if (_entries.ContainsKey(key)) return false;
        _entries[key] = new Entry(value, null);
        return true;
    }

    public bool Expire(string key, TimeSpan ttl)
    {
        RequireLiveClient();
        if (ttl <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(ttl));
        PurgeExpired(key);
        if (!_entries.TryGetValue(key, out var entry)) return false;
        _entries[key] = entry with { ExpiresAt = _clock.UtcNow + ttl };
        return true;
    }

    // Một lệnh điều kiện có expiry, mô phỏng hợp đồng atomic của Redis.
    public bool SetIfAbsentWithExpiry(string key, string value, TimeSpan ttl)
    {
        RequireLiveClient();
        if (ttl <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(ttl));
        PurgeExpired(key);
        if (_entries.ContainsKey(key)) return false;
        _entries[key] = new Entry(value, _clock.UtcNow + ttl);
        return true;
    }

    public bool Exists(string key)
    {
        RequireLiveClient();
        PurgeExpired(key);
        return _entries.ContainsKey(key);
    }

    public string? Get(string key)
    {
        RequireLiveClient();
        PurgeExpired(key);
        return _entries.TryGetValue(key, out var entry) ? entry.Value : null;
    }

    public TimeSpan? TimeToLive(string key)
    {
        RequireLiveClient();
        PurgeExpired(key);
        if (!_entries.TryGetValue(key, out var entry) || entry.ExpiresAt is null) return null;
        return entry.ExpiresAt.Value - _clock.UtcNow;
    }
}

public sealed class SimulatedCrashException : Exception
{
    public SimulatedCrashException() : base("Client process terminated after durable store operation.") { }
}

public sealed class CrashInjector
{
    private readonly SimulatedRedis _redis;
    public CrashInjector(SimulatedRedis redis) => _redis = redis;
    public bool CrashAfterNextPersist { get; set; }

    public void AfterPersist()
    {
        if (!CrashAfterNextPersist) return;
        CrashAfterNextPersist = false;
        _redis.CrashClient();
        throw new SimulatedCrashException();
    }
}
