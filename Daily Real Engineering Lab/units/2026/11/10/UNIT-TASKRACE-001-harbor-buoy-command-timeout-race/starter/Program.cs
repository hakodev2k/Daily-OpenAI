using System;
using System.Threading;
using System.Threading.Tasks;

var station = new BuoyStation();
var firstGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var first = station.SendWithTimeoutAsync("MAINTENANCE", firstGate.Task, Task.CompletedTask);
Console.WriteLine("first outcome: " + await first);
var second = station.SendWithTimeoutAsync("ACTIVE", Task.CompletedTask, Task.CompletedTask);
Console.WriteLine("second outcome: " + await second);
Console.WriteLine("mode before old ack: " + station.Mode);
firstGate.SetResult();
await station.WaitForAllAsync();
Console.WriteLine("mode after old ack: " + station.Mode);
Console.WriteLine("expected final: ACTIVE");

sealed class BuoyStation
{
    private readonly System.Collections.Generic.List<Task> _pending = new();
    public string Mode { get; private set; } = "IDLE";

    public async Task<string> SendWithTimeoutAsync(string requestedMode, Task ack, Task deadline)
    {
        var operation = ApplyWhenAcknowledgedAsync(requestedMode, ack);
        _pending.Add(operation);
        var winner = await Task.WhenAny(operation, deadline);
        if (winner == deadline) return "TIMEOUT";
        await operation;
        return "OK";
    }

    private async Task ApplyWhenAcknowledgedAsync(string mode, Task ack)
    {
        await ack;
        Mode = mode;
    }

    public Task WaitForAllAsync() => Task.WhenAll(_pending);
}
