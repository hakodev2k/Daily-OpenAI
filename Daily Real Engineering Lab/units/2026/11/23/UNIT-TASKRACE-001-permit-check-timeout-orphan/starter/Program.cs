using System;
using System.Threading;
using System.Threading.Tasks;

var mode = args.Length == 0 ? "reproduce" : args[0];
var service = new PermitService();
var result = await service.CheckWithDeadlineAsync("permit-42", TimeSpan.FromMilliseconds(30));
Console.WriteLine($"response={result}; atDeadline={service.CompletedWrites}");
await Task.Delay(250);
Console.WriteLine($"afterGrace={service.CompletedWrites}");
bool orphan = result == "TIMEOUT" && service.CompletedWrites > 0;
if (mode == "reproduce") {
    Console.WriteLine(orphan ? "REPRODUCED: late write after timeout" : "NOT_REPRODUCED");
    return orphan ? 0 : 1;
}
if (mode == "verify") {
    Console.WriteLine(!orphan && result == "TIMEOUT" && service.CompletedWrites == 0 ? "PASS" : "FAIL");
    return !orphan && result == "TIMEOUT" && service.CompletedWrites == 0 ? 0 : 1;
}
Console.Error.WriteLine("Use reproduce or verify");
return 2;

sealed class PermitService {
    private int _completedWrites;
    public int CompletedWrites => Volatile.Read(ref _completedWrites);

    public async Task<string> CheckWithDeadlineAsync(string permitId, TimeSpan deadline) {
        var operation = ProcessAsync(permitId, CancellationToken.None);
        var winner = await Task.WhenAny(operation, Task.Delay(deadline));
        if (winner != operation) return "TIMEOUT";
        return await operation;
    }

    private async Task<string> ProcessAsync(string permitId, CancellationToken token) {
        await Task.Delay(180, token);
        Interlocked.Increment(ref _completedWrites);
        return "APPROVED";
    }
}
