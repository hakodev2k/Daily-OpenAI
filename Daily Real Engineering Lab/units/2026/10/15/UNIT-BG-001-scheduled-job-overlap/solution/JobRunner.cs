namespace Lab;

public sealed class JobRunner
{
    private readonly Func<CancellationToken, Task> _processBatch;
    private readonly TimeSpan _interval;
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _loop;

    public JobRunner(Func<CancellationToken, Task> processBatch, TimeSpan interval)
    {
        _processBatch = processBatch ?? throw new ArgumentNullException(nameof(processBatch));
        if (interval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(interval));
        _interval = interval;
    }

    public void Start()
    {
        if (_loop is not null) throw new InvalidOperationException("Runner already started.");
        _loop = RunAsync(_shutdown.Token);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _processBatch(cancellationToken);
            using var timer = new PeriodicTimer(_interval);
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await _processBatch(cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal shutdown: no new batch is scheduled.
        }
    }

    public async Task StopAsync()
    {
        _shutdown.Cancel();
        if (_loop is not null) await _loop;
    }
}
