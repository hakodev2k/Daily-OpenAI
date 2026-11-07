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
        if (_slots.CurrentCount == 0)
        {
            return new AdmissionResult(requestId, false, 0);
        }

        await _observer.AfterCapacityObservationAsync(cancellationToken);

        var stopwatch = Stopwatch.StartNew();
        await _slots.WaitAsync(cancellationToken);
        stopwatch.Stop();

        try
        {
            await work(cancellationToken);
            return new AdmissionResult(requestId, true, stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            _slots.Release();
        }
    }
}
