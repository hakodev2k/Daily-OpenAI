namespace AlertDispatchLab;

public sealed record Alert(string AlertId, string Facility);

public interface IAlertGateway
{
    Task SendAsync(Alert alert, CancellationToken cancellationToken);
}

public sealed class DispatchService
{
    public Task DispatchBatchAsync(
        IReadOnlyList<Alert> alerts,
        IAlertGateway gateway,
        CancellationToken cancellationToken)
    {
        alerts.ToList().ForEach(async alert =>
        {
            await gateway.SendAsync(alert, cancellationToken);
        });

        return Task.CompletedTask;
    }
}
