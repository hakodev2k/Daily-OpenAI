using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

var gate = new TwoRequestGate();
var audit = new ConcurrentQueue<string>();
var middleware = new TenantAuditMiddleware(async _ => await gate.ArriveAndWaitAsync(), audit);

var requestA = NewContext("request-a", "tenant-a");
var requestB = NewContext("request-b", "tenant-b");

var taskA = middleware.InvokeAsync(requestA);
await gate.WaitForFirstArrivalAsync();
var taskB = middleware.InvokeAsync(requestB);
await Task.WhenAll(taskA, taskB);

foreach (var row in audit.OrderBy(x => x))
    Console.WriteLine(row);

static DefaultHttpContext NewContext(string requestId, string tenant)
{
    var context = new DefaultHttpContext();
    context.TraceIdentifier = requestId;
    context.Request.Headers["X-Tenant"] = tenant;
    return context;
}

sealed class TenantAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentQueue<string> _audit;

    public TenantAuditMiddleware(RequestDelegate next, ConcurrentQueue<string> audit)
    {
        _next = next;
        _audit = audit;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenant = context.Request.Headers["X-Tenant"].ToString();
        await _next(context);
        _audit.Enqueue($"{context.TraceIdentifier} -> {tenant}");
    }
}

sealed class TwoRequestGate
{
    private readonly TaskCompletionSource _firstArrived = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _bothArrived = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _count;

    public Task WaitForFirstArrivalAsync() => _firstArrived.Task;

    public Task ArriveAndWaitAsync()
    {
        var count = Interlocked.Increment(ref _count);
        if (count == 1) _firstArrived.TrySetResult();
        if (count == 2) _bothArrived.TrySetResult();
        return _bothArrived.Task;
    }
}
