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

        try
        {
            await _gpuSlots.WaitAsync(cancellationToken).ConfigureAwait(false);
            await render(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gpuSlots.Release();
        }
    }
}
