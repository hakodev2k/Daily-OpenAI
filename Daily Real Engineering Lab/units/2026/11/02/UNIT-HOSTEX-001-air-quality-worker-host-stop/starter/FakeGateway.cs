public sealed class FakeGateway(DeliveryJournal journal)
{
    public async Task PublishAsync(string alertId, CancellationToken cancellationToken)
    {
        journal.Attempts.Enqueue(alertId);
        await Task.Delay(20, cancellationToken);

        if (alertId == "AQ-102")
        {
            throw new PartnerGatewayException("Partner returned 503 for AQ-102");
        }
    }
}

public sealed class PartnerGatewayException(string message) : Exception(message);
