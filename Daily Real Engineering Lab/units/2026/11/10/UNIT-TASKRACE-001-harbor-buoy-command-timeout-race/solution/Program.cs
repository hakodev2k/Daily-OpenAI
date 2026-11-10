using System;
using System.Threading;
using System.Threading.Tasks;

var station = new BuoyStation();
var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
Console.WriteLine("first outcome: " + await station.SendWithTimeoutAsync("MAINTENANCE", gate.Task, Task.CompletedTask));
Console.WriteLine("second outcome: " + await station.SendWithTimeoutAsync("ACTIVE", Task.CompletedTask, Task.CompletedTask));
gate.SetResult();
await station.WaitForAllAsync();
Console.WriteLine("mode after old ack: " + station.Mode);
if (station.Mode != "ACTIVE") throw new Exception("Stale command won");

sealed class BuoyStation
{
    private readonly System.Collections.Generic.List<Task> _pending = new();
    private long _generation;
    public string Mode { get; private set; } = "IDLE";
    public async Task<string> SendWithTimeoutAsync(string requestedMode, Task ack, Task deadline)
    {
        var generation = Interlocked.Increment(ref _generation);
        var operation = ApplyWhenAcknowledgedAsync(requestedMode, ack, generation);
        _pending.Add(operation);
        var winner = await Task.WhenAny(operation, deadline);
        if (winner == deadline) return "TIMEOUT";
        await operation;
        return "OK";
    }
    private async Task ApplyWhenAcknowledgedAsync(string mode, Task ack, long generation)
    {
        await ack;
        // A late acknowledgment cannot supersede a more recent request.
        if (generation == Interlocked.Read(ref _generation)) Mode = mode;
    }
    public Task WaitForAllAsync() => Task.WhenAll(_pending);
}
