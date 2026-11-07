namespace Lab;

public sealed class JobRunner
{
    private readonly Func<CancellationToken, Task> _processBatch;
    private readonly TimeSpan _interval;
    private Timer? _timer;

    public JobRunner(Func<CancellationToken, Task> processBatch, TimeSpan interval)
    {
        _processBatch = processBatch ?? throw new ArgumentNullException(nameof(processBatch));
        if (interval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(interval));
        _interval = interval;
    }

    public void Start()
    {
        if (_timer is not null) throw new InvalidOperationException("Runner already started.");
        _timer = new Timer(async _ =>
        {
            await _processBatch(CancellationToken.None);
        }, null, TimeSpan.Zero, _interval);
    }

    public Task StopAsync()
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }
}
