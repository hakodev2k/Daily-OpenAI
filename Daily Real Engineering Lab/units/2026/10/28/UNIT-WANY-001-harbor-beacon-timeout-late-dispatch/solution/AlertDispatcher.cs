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

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var pending = _transport.SendAsync(alertId, linked.Token);
        var completed = await Task.WhenAny(pending, deadlineSignal);

        if (completed == pending)
        {
            await pending; // Preserve transport faults and caller cancellation.
            return DispatchOutcome.Delivered;
        }

        linked.Cancel();
        try
        {
            await pending; // Observe the final outcome before returning timeout.
        }
        catch (OperationCanceledException) when (linked.IsCancellationRequested)
        {
            // Cooperative cancellation confirmed by the transport.
        }
        return DispatchOutcome.TimedOut;
    }
}
