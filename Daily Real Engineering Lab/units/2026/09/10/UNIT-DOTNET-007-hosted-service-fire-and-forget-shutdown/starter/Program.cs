using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<Progress>();
builder.Services.AddHostedService<AuditExportWorker>();
using var host = builder.Build();

await host.StartAsync();
await Task.Delay(100);
Console.WriteLine("host: shutdown requested");
await host.StopAsync(TimeSpan.FromSeconds(2));

var progress = host.Services.GetRequiredService<Progress>();
Console.WriteLine($"summary: started={progress.Started} completed={progress.Completed}");

sealed class Progress
{
    private int _started;
    private int _completed;
    public int Started => Volatile.Read(ref _started);
    public int Completed => Volatile.Read(ref _completed);
    public void MarkStarted() => Interlocked.Increment(ref _started);
    public void MarkCompleted() => Interlocked.Increment(ref _completed);
}

sealed class AuditExportWorker(Progress progress) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("worker: ExecuteAsync entered");

        foreach (var jobId in new[] { 101, 102, 103 })
        {
            _ = ProcessAsync(jobId);
        }

        Console.WriteLine("worker: ExecuteAsync returning");
        return Task.CompletedTask;
    }

    private async Task ProcessAsync(int jobId)
    {
        progress.MarkStarted();
        Console.WriteLine($"job {jobId}: started");
        await Task.Delay(500);
        progress.MarkCompleted();
        Console.WriteLine($"job {jobId}: completed");
    }
}
