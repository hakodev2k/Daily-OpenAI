using System.Runtime.CompilerServices;
using System.Threading;
var mode = args.FirstOrDefault() ?? "reproduce";
using var cancelled = new CancellationTokenSource();
cancelled.Cancel();
int stopped = 0, healthy = 0;
try {
    await foreach (var sample in Feed.Read().WithCancellation(cancelled.Token)) {
        stopped++;
        Console.WriteLine($"cancelled sample {sample}");
    }
} catch (OperationCanceledException) { Console.WriteLine("Cancellation observed"); }
await foreach (var sample in Feed.Read().WithCancellation(CancellationToken.None)) healthy++;
Console.WriteLine($"evidence cancelled={stopped} healthy={healthy}");
if (mode == "reproduce" && stopped == 3 && healthy == 3) { Console.WriteLine("REPRODUCED"); return 0; }
if (mode == "verify" && stopped == 0 && healthy == 3) { Console.WriteLine("PASS"); return 0; }
Console.Error.WriteLine("Contract check failed"); return 1;
static class Feed {
    public static async IAsyncEnumerable<int> Read([EnumeratorCancellation] CancellationToken token = default) {
        for (int i = 1; i <= 3; i++) {
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            yield return i;
        }
    }
}
