namespace HarborAlertLab;

public enum DispatchOutcome { Delivered, TimedOut }

public interface IAlertTransport
{
    Task SendAsync(string alertId, CancellationToken cancellationToken);
}

public sealed class AlertDispatcher
{
    private readonly IAlertTransport _transport;

    public AlertDispatcher(IAlertTransport transport) => _transport = transport;

    public async Task<DispatchOutcome> SendAsync(
        string alertId,
        Task deadlineSignal,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alertId);
        ArgumentNullException.ThrowIfNull(deadlineSignal);

        var pending = _transport.SendAsync(alertId, cancellationToken);
        var completed = await Task.WhenAny(pending, deadlineSignal);

        if (completed == deadlineSignal)
            return DispatchOutcome.TimedOut;

        await pending;
        return DispatchOutcome.Delivered;
    }
}
