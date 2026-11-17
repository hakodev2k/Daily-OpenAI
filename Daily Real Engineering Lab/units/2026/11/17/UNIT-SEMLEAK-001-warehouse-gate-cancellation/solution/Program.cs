using System.Threading;

var gate = new LoadingGate();
using var cancelB = new CancellationTokenSource();
var enteredA = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var releaseA = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var a = gate.ProcessAsync("A", async () => { enteredA.SetResult(); await releaseA.Task; }, CancellationToken.None);
await enteredA.Task;
Console.WriteLine("A entered");
var b = gate.ProcessAsync("B", () => Task.CompletedTask, cancelB.Token);
cancelB.Cancel();
try { await b; } catch (OperationCanceledException) { Console.WriteLine("B cancelled"); }
var cEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var c = gate.ProcessAsync("C", () => { cEntered.SetResult(); return Task.CompletedTask; }, CancellationToken.None);
var premature = await Task.WhenAny(cEntered.Task, Task.Delay(150)) == cEntered.Task;
Console.WriteLine(premature ? "VIOLATION: C entered before A left" : "C still waiting");
releaseA.SetResult();
await Task.WhenAll(a, c);
if (premature || gate.MaxActive > 1) { Console.WriteLine($"FAIL maxActive={gate.MaxActive}"); Environment.ExitCode = 1; }
else Console.WriteLine("PASS maxActive=1");

sealed class LoadingGate
{
    private readonly SemaphoreSlim _slots = new(1, 1);
    private int _active;
    private int _maxActive;
    public int MaxActive => Volatile.Read(ref _maxActive);
    public async Task ProcessAsync(string name, Func<Task> work, CancellationToken token)
    {
        await _slots.WaitAsync(token);
        try
        {
            var now = Interlocked.Increment(ref _active);
            if (now > _maxActive) Interlocked.Exchange(ref _maxActive, now);
            try { await work(); }
            finally { Interlocked.Decrement(ref _active); }
        }
        finally { _slots.Release(); }
    }
}
