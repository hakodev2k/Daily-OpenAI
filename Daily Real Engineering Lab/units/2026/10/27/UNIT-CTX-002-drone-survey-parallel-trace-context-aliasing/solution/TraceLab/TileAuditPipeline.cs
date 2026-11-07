namespace TraceLab;

public sealed record AuditRow(string TileId, string AuditTileId);

public sealed class TraceState
{
    public string TileId { get; set; } = "";
}

public static class TileAuditPipeline
{
    private static readonly AsyncLocal<TraceState?> Current = new();

    public static async Task<AuditRow[]> ProcessAsync(IReadOnlyList<string> tileIds)
    {
        if (tileIds.Count == 0) return Array.Empty<AuditRow>();

        Current.Value = new TraceState { TileId = "SURVEY-BATCH" };
        var allPrepared = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var preparedCount = 0;

        var tasks = tileIds.Select(async tileId =>
        {
            await Task.Yield();
            Current.Value = new TraceState { TileId = tileId };

            if (Interlocked.Increment(ref preparedCount) == tileIds.Count)
                allPrepared.TrySetResult(true);

            await allPrepared.Task;
            return new AuditRow(tileId, Current.Value!.TileId);
        }).ToArray();

        return await Task.WhenAll(tasks);
    }
}
