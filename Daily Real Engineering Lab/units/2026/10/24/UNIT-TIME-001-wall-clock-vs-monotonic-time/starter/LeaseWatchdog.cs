namespace ClockLab;

/// <summary>
/// Giới hạn thời lượng một batch. Chỉ sửa file này trong lab.
/// </summary>
public sealed class LeaseWatchdog
{
    private readonly TimeProvider _clock;
    private readonly TimeSpan _budget;
    private DateTimeOffset? _startedAt;

    public LeaseWatchdog(TimeProvider clock, TimeSpan budget)
    {
        if (budget <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(budget));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _budget = budget;
    }

    public void Start() => _startedAt = _clock.GetUtcNow();

    public bool IsExpired()
    {
        if (_startedAt is null)
            throw new InvalidOperationException("Watchdog has not been started.");
        return _clock.GetUtcNow() - _startedAt.Value >= _budget;
    }
}
