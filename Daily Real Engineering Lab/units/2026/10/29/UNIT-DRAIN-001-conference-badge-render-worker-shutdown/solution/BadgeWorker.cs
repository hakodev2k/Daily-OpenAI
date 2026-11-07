namespace BadgeDrainLab;

internal static class BadgeWorker
{
    public static async Task HandleOneAsync(
        JobStore store, BadgeRenderer renderer, CancellationToken cancellationToken)
    {
        var jobId = store.Lease();
        if (jobId is null) return;

        try
        {
            await renderer.RenderAsync(jobId, cancellationToken);
            store.Acknowledge(jobId);
        }
        catch
        {
            store.Release(jobId);
            throw;
        }
    }
}
