using System.Collections.Concurrent;
using System.Threading.Channels;

public sealed class ProjectionWorker(ChannelReader<int> reader, ConcurrentBag<int> completed)
{
    public int Received { get; private set; }

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        await foreach (var jobId in reader.ReadAllAsync(stoppingToken))
        {
            Received++;

            // Investigation note:
            // At this point, who owns the lifetime of the operation that was just started?
            _ = ProcessAsync(jobId);
        }
    }

    private async Task ProcessAsync(int jobId)
    {
        await Task.Delay(250);
        completed.Add(jobId);
        Console.WriteLine($"Completed {jobId}");
    }
}
