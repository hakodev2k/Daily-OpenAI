namespace PermitGateLab;

public sealed class WorkGate
{
    private readonly SemaphoreSlim _slots;

    public WorkGate(int maxWorkers)
    {
        if (maxWorkers < 1) throw new ArgumentOutOfRangeException(nameof(maxWorkers));
        _slots = new SemaphoreSlim(maxWorkers);
    }

    public async Task RunAsync(Func<CancellationToken, Task> render, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(render);

        try
        {
            await _slots.WaitAsync(cancellationToken);
            await render(cancellationToken);
        }
        finally
        {
            _slots.Release();
        }
    }
}
