namespace OrbitTileLab;

public sealed class TileGate
{
    private readonly SemaphoreSlim _gpuSlots = new(initialCount: 1, maxCount: 1);

    public async Task RenderAsync(
        string tileId,
        Func<CancellationToken, Task> render,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tileId);
        ArgumentNullException.ThrowIfNull(render);

        bool acquired = false;
        try
        {
            await _gpuSlots.WaitAsync(cancellationToken).ConfigureAwait(false);
            acquired = true;
            await render(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (acquired)
            {
                _gpuSlots.Release();
            }
        }
    }
}
