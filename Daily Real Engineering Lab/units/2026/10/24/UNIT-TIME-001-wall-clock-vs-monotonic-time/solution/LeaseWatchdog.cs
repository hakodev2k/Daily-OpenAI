namespace ClockLab;

/// <summary>Reference Solution — xem sau khi tự điều tra.</summary>
public sealed class LeaseWatchdog
{
    private readonly TimeProvider _clock;
    private readonly TimeSpan _budget;
    private long? _startTimestamp;

    public LeaseWatchdog(TimeProvider clock, TimeSpan budget)
    {
        if (budget <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(budget));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _budget = budget;
    }

    public void Start() => _startTimestamp = _clock.GetTimestamp();

    public bool IsExpired()
    {
        if (_startTimestamp is null)
            throw new InvalidOperationException("Watchdog has not been started.");
        return _clock.GetElapsedTime(_startTimestamp.Value) >= _budget;
    }
}
