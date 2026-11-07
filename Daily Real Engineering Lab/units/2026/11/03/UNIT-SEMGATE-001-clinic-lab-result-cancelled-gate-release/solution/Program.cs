using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ResultDispatchGate
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    public int Available => _gate.CurrentCount;

    public async Task DispatchAsync(Func<Task> send, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await send();
        }
        finally
        {
            _gate.Release();
        }
    }
}

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var gate = new ResultDispatchGate();
        var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var active = gate.DispatchAsync(async () =>
        {
            entered.TrySetResult(true);
            await release.Task;
        }, CancellationToken.None);
        await entered.Task;
        if (gate.Available != 0) throw new Exception("Fixture failed: gate not occupied");

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        bool observedCancellation = false;
        try { await gate.DispatchAsync(() => Task.CompletedTask, cancelled.Token); }
        catch (OperationCanceledException) { observedCancellation = true; }
        if (!observedCancellation) throw new Exception("Cancellation was not observed");

        var availableWhileBusy = gate.Available;
        Console.WriteLine($"available_while_busy={availableWhileBusy}");

        release.TrySetResult(true);
        await active;
        var availableAfter = gate.Available;
        Console.WriteLine($"available_after={availableAfter}");

        bool healthy = availableWhileBusy == 0 && availableAfter == 1;
        if (args.Length == 1 && args[0] == "--reproduce")
        {
            if (healthy) { Console.Error.WriteLine("Symptom not reproduced"); return 1; }
            Console.WriteLine("REPRODUCED: cancellation changed gateway capacity");
            return 0;
        }
        if (args.Length == 1 && args[0] == "--verify")
        {
            if (!healthy) { Console.Error.WriteLine("FAIL: cancellation corrupted capacity"); return 1; }
            Console.WriteLine("PASS: cancellation preserves gateway capacity");
            return 0;
        }
        Console.Error.WriteLine("Usage: --reproduce or --verify");
        return 2;
    }
}
