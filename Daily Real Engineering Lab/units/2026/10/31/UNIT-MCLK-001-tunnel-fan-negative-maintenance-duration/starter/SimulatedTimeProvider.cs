namespace ClockLab;

// Deterministic fixture. Keep unchanged during the learner exercise.
public sealed class SimulatedTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;
    private long _timestamp;

    public SimulatedTimeProvider(DateTimeOffset initialUtc)
    {
        if (initialUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("UTC offset must be zero.", nameof(initialUtc));
        _utcNow = initialUtc;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;
    public override long GetTimestamp() => _timestamp;
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public void Advance(TimeSpan elapsed, TimeSpan wallClockAdjustment)
    {
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        _timestamp = checked(_timestamp + elapsed.Ticks);
        _utcNow = _utcNow.Add(elapsed).Add(wallClockAdjustment);
    }
}
