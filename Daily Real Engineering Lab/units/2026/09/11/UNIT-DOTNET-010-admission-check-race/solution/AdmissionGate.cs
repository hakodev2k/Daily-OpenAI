using System.Diagnostics;

public sealed record AdmissionResult(string RequestId, bool Accepted, long WaitMilliseconds);

public interface IAdmissionObserver
{
    Task AfterCapacityObservationAsync(CancellationToken cancellationToken);
}

public sealed class NoopAdmissionObserver : IAdmissionObserver
{
    public Task AfterCapacityObservationAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public sealed class AdmissionGate
{
    private readonly SemaphoreSlim _slots;
    private readonly IAdmissionObserver _observer;

    public AdmissionGate(int capacity, IAdmissionObserver observer)
    {
        _slots = new SemaphoreSlim(capacity, capacity);
        _observer = observer;
    }

    public async Task<AdmissionResult> TryRunAsync(
        string requestId,
        Func<CancellationToken, Task> work,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var acquired = await _slots.WaitAsync(0, cancellationToken);
        stopwatch.Stop();

        if (!acquired)
        {
            return new AdmissionResult(requestId, false, stopwatch.ElapsedMilliseconds);
        }

        try
        {
            await _observer.AfterCapacityObservationAsync(cancellationToken);
            await work(cancellationToken);
            return new AdmissionResult(requestId, true, stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            _slots.Release();
        }
    }
}
