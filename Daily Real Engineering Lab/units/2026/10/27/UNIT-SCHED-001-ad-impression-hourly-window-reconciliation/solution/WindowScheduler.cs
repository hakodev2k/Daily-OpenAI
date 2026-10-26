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

    public async Task<int> OnWakeupAsync(
        DateTimeOffset now,
        IWindowSink sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var completed = 0;
        while (LastCompletedWindow + _interval <= now)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var windowEnd = LastCompletedWindow + _interval;
            await sink.ProcessAsync(windowEnd, cancellationToken);
            LastCompletedWindow = windowEnd; // Commit only after successful processing.
            completed++;
        }

        return completed;
    }
}
