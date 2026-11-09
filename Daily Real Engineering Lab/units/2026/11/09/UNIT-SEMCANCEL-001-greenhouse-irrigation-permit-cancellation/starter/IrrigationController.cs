using System.Threading;

namespace IrrigationGate;

public sealed class IrrigationController
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        try
        {
            await _gate.WaitAsync(cancellationToken);
            await action();
        }
        finally
        {
            _gate.Release();
        }
    }
}
