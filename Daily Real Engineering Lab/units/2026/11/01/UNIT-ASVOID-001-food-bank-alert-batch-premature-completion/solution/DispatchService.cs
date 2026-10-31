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
        Task[] sends = alerts
            .Select(alert => gateway.SendAsync(alert, cancellationToken))
            .ToArray();

        return Task.WhenAll(sends);
    }
}
