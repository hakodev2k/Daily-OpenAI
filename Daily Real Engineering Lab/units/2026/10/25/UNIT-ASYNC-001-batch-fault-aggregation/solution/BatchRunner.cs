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

        var pending = jobs.Select(job =>
        {
            Task<int> task;
            try
            {
                task = job.FetchAsync(cancellationToken) ??
                    Task.FromException<int>(new InvalidOperationException("Adapter returned null Task"));
            }
            catch (Exception ex)
            {
                // An adapter may throw before returning its Task.
                task = Task.FromException<int>(ex);
            }
            return (job.Partner, Task: task);
        }).ToArray();

        try
        {
            await Task.WhenAll(pending.Select(item => item.Task));
        }
        catch (Exception)
        {
            // All tasks have reached a terminal state. Inspect each below.
            // Do not rely on the one exception surfaced by await.
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (pending.Any(item => item.Task.IsCanceled))
            throw new OperationCanceledException("A source canceled the batch.", cancellationToken);

        var successes = new List<FeedSuccess>();
        var failures = new List<FeedFailure>();

        foreach (var item in pending)
        {
            if (item.Task.IsCompletedSuccessfully)
            {
                successes.Add(new FeedSuccess(item.Partner, item.Task.Result));
            }
            else if (item.Task.IsFaulted)
            {
                foreach (Exception ex in item.Task.Exception!.Flatten().InnerExceptions)
                    failures.Add(new FeedFailure(item.Partner, ex.GetType().Name, ex.Message));
            }
        }

        return new BatchReport(successes, failures);
    }
}
