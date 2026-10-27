namespace Lab;

public enum DispatchOutcome
{
    Executed,
    Busy
}

public sealed class DispatchCoordinator
{
    private readonly SemaphoreSlim _deviceGate = new(initialCount: 1, maxCount: 1);

    public async Task<DispatchOutcome> DispatchAsync(
        Func<Task> sendCommand,
        TimeSpan admissionWait,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sendCommand);

        await _deviceGate.WaitAsync(admissionWait, cancellationToken);

        try
        {
            await sendCommand();
            return DispatchOutcome.Executed;
        }
        finally
        {
            _deviceGate.Release();
        }
    }
}
