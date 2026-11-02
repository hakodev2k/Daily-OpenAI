using Microsoft.Extensions.Hosting;

public sealed class NoticeWorker(DeliveryJournal journal, FakeGateway gateway) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var alertId in new[] { "AQ-101", "AQ-102", "AQ-103" })
        {
            await Task.Delay(35, stoppingToken);
            try
            {
                await gateway.PublishAsync(alertId, stoppingToken);
                journal.Published.Enqueue(alertId);
            }
            catch (PartnerGatewayException ex) when (!stoppingToken.IsCancellationRequested)
            {
                journal.Failed.Enqueue(alertId);
                Console.WriteLine($"Recoverable delivery failure for {alertId}: {ex.Message}");
            }
        }

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
