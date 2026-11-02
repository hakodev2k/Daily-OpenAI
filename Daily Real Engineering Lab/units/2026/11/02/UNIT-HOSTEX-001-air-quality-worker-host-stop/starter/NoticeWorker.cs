using Microsoft.Extensions.Hosting;

public sealed class NoticeWorker(DeliveryJournal journal, FakeGateway gateway) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var alertId in new[] { "AQ-101", "AQ-102", "AQ-103" })
        {
            await Task.Delay(35, stoppingToken);
            await gateway.PublishAsync(alertId, stoppingToken);
            journal.Published.Enqueue(alertId);
        }

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
