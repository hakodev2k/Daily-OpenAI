using System.Collections.Concurrent;
using System.Diagnostics;

var store = new ConcurrentDictionary<string, string>();
var clock = Stopwatch.StartNew();

async Task<bool> ImportAsync(string key, int delayMs, int timeoutMs)
{
    using var cts = new CancellationTokenSource(timeoutMs);
    try
    {
        await Task.Delay(delayMs, cts.Token);
        cts.Token.ThrowIfCancellationRequested();
        // In production, commit needs an atomic, idempotent persistence boundary.
        store.TryAdd(key, "indexed");
        Console.WriteLine($"WRITE {key} at {clock.ElapsedMilliseconds}ms");
        Console.WriteLine($"OK {key}");
        return true;
    }
    catch (OperationCanceledException) when (cts.IsCancellationRequested)
    {
        Console.WriteLine($"TIMEOUT {key} at {clock.ElapsedMilliseconds}ms");
        return false;
    }
}

await ImportAsync("slow", 220, 80);
Console.WriteLine($"AT_RESPONSE {store.Count}");
await Task.Delay(300);
Console.WriteLine($"AFTER_WAIT {store.Count}");
await ImportAsync("fast", 5, 80);
Console.WriteLine($"FINAL {store.Count}");
