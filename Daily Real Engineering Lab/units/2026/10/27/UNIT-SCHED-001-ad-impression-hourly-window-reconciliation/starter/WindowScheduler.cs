namespace AdWindowLab;

public interface IWindowSink
{
    Task ProcessAsync(DateTimeOffset windowEnd, CancellationToken cancellationToken);
}

public sealed class WindowScheduler
{
    private readonly TimeSpan _interval;

    public WindowScheduler(DateTimeOffset initialCheckpoint, TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(interval));

        LastCompletedWindow = initialCheckpoint;
        _interval = interval;
    }

    public DateTimeOffset LastCompletedWindow { get; private set; }

    // Invoked whenever the worker receives a periodic wake-up.
    public async Task<int> OnWakeupAsync(
        DateTimeOffset now,
        IWindowSink sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var nextWindow = LastCompletedWindow + _interval;
        if (nextWindow > now)
            return 0;

        await sink.ProcessAsync(nextWindow, cancellationToken);
        LastCompletedWindow = nextWindow;
        return 1;
    }
}
