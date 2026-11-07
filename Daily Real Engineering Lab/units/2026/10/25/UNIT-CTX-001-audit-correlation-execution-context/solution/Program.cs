using System.Threading;

var mode = args.FirstOrDefault() ?? "--inspect";
if (mode is not ("--inspect" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --inspect | --reproduce | --verify");
    return 2;
}

// Hai yêu cầu độc lập chạy song song, mỗi yêu cầu có một correlation ID riêng.
var results = await Task.WhenAll(
    SimulateRequestAsync("CASE-A", "corr-risk-A-2042"),
    SimulateRequestAsync("CASE-B", "corr-risk-B-2043"));

foreach (var item in results)
{
    Console.WriteLine(
        $"event={item.Receipt.EventId} expected={item.Expected} " +
        $"control={item.Control} audit={item.Receipt.CorrelationId}");
}

var controlsCorrect = results.All(x => x.Control == x.Expected);
var auditsCorrect = results.All(x => x.Receipt.CorrelationId == x.Expected);
if (mode == "--reproduce")
{
    var observed = controlsCorrect && results.All(x => x.Receipt.CorrelationId == "missing");
    Console.WriteLine(observed ? "REPRODUCED: audit context missing" : "NOT REPRODUCED");
    return observed ? 0 : 1;
}
if (mode == "--verify")
{
    var verified = controlsCorrect && auditsCorrect;
    Console.WriteLine(verified ? "PASS: isolated audit correlation" : "FAIL: audit correlation contract");
    return verified ? 0 : 1;
}
return 0;

static async Task<RequestResult> SimulateRequestAsync(string eventId, string expected)
{
    RequestContext.CorrelationId.Value = expected;
    try
    {
        // Đường kiểm soát: một thao tác async thông thường.
        var control = await Task.Run(() => RequestContext.CorrelationId.Value ?? "missing");
        var receipt = await AuditDispatcher.DispatchAsync(new AuditJob(eventId, "risk-evaluation"));
        return new RequestResult(expected, control, receipt);
    }
    finally
    {
        RequestContext.CorrelationId.Value = null;
    }
}

internal static class RequestContext
{
    internal static readonly AsyncLocal<string?> CorrelationId = new();
}

internal sealed record AuditJob(string EventId, string Category);
internal sealed record AuditReceipt(string EventId, string CorrelationId, string Category);
internal sealed record RequestResult(string Expected, string Control, AuditReceipt Receipt);
internal sealed record WorkState(AuditJob Job, string CorrelationAtDispatch, TaskCompletionSource<AuditReceipt> Completion);

internal static class AuditDispatcher
{
    internal static Task<AuditReceipt> DispatchAsync(AuditJob job)
    {
        var completion = new TaskCompletionSource<AuditReceipt>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        ThreadPool.UnsafeQueueUserWorkItem(static (WorkState state) =>
        {
            try
            {
                var correlationId = state.CorrelationAtDispatch;
                state.Completion.TrySetResult(
                    new AuditReceipt(state.Job.EventId, correlationId, state.Job.Category));
            }
            catch (Exception ex)
            {
                state.Completion.TrySetException(ex);
            }
        }, new WorkState(job, RequestContext.CorrelationId.Value ?? "missing", completion), preferLocal: false);

        return completion.Task;
    }
}
