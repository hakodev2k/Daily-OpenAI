namespace ClockLab;

/// <summary>
/// Single-threaded deterministic simulator; không chỉnh đồng hồ host.
/// </summary>
public sealed class ManualTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;
    private long _timestamp;

    public ManualTimeProvider(DateTimeOffset initialUtc)
    {
        if (initialUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Expected UTC offset 00:00.", nameof(initialUtc));
        _utcNow = initialUtc;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;
    public override long GetTimestamp() => _timestamp;
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public void Advance(TimeSpan elapsed)
    {
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        checked { _timestamp += elapsed.Ticks; }
        _utcNow = _utcNow.Add(elapsed);
    }

    public void AdjustWallClock(TimeSpan adjustment) =>
        _utcNow = _utcNow.Add(adjustment);
}
