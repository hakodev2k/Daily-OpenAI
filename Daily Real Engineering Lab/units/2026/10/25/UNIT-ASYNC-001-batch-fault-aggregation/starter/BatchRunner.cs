namespace BatchFaultLab;

public sealed record FeedJob(string Partner, Func<CancellationToken, Task<int>> FetchAsync);
public sealed record FeedSuccess(string Partner, int Rows);
public sealed record FeedFailure(string Partner, string ErrorType, string Message);
public sealed record BatchReport(IReadOnlyList<FeedSuccess> Succeeded, IReadOnlyList<FeedFailure> Failed);

public static class BatchRunner
{
    public static async Task<BatchReport> RunAsync(
        IReadOnlyList<FeedJob> jobs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Each source is independent. Operations may complete or fail separately.
        var pending = jobs
            .Select(job => (job.Partner, Task: job.FetchAsync(cancellationToken)))
            .ToArray();

        try
        {
            await Task.WhenAll(pending.Select(item => item.Task));
        }
        catch (Exception ex)
        {
            return new BatchReport(
                Array.Empty<FeedSuccess>(),
                new[] { new FeedFailure("batch", ex.GetType().Name, ex.Message) });
        }

        return new BatchReport(
            pending.Select(item => new FeedSuccess(item.Partner, item.Task.Result)).ToArray(),
            Array.Empty<FeedFailure>());
    }
}
