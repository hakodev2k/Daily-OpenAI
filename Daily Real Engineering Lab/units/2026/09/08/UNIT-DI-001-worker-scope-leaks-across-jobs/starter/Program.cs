using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddScoped<JobContext>();
services.AddScoped<OrderReconciliationHandler>();

using var provider = services.BuildServiceProvider(validateScopes: true);
var jobs = new[] { "JOB-1001", "JOB-1002", "JOB-1003" };

using var batchScope = provider.CreateScope();
var handler = batchScope.ServiceProvider.GetRequiredService<OrderReconciliationHandler>();

foreach (var jobId in jobs)
{
    handler.Handle(jobId);
}

public sealed class JobContext
{
    private static int _nextId;

    public JobContext()
    {
        ContextId = Interlocked.Increment(ref _nextId);
    }

    public int ContextId { get; }
}

public sealed class OrderReconciliationHandler(JobContext context)
{
    public void Handle(string jobId)
    {
        Console.WriteLine($"JobId={jobId};ContextId={context.ContextId}");
    }
}
