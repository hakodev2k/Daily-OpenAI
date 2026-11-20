using System.Collections.Concurrent;
using System.Diagnostics;

var store = new ConcurrentDictionary<string, string>();
var clock = Stopwatch.StartNew();

async Task<bool> ImportAsync(string key, int delayMs, int timeoutMs)
{
    async Task SaveAsync()
    {
        await Task.Delay(delayMs);
        store.TryAdd(key, "indexed");
        Console.WriteLine($"WRITE {key} at {clock.ElapsedMilliseconds}ms");
    }

    var work = SaveAsync();
    var winner = await Task.WhenAny(work, Task.Delay(timeoutMs));
    if (winner != work)
    {
        Console.WriteLine($"TIMEOUT {key} at {clock.ElapsedMilliseconds}ms");
        return false;
    }
    await work;
    Console.WriteLine($"OK {key}");
    return true;
}

await ImportAsync("slow", 220, 80);
Console.WriteLine($"AT_RESPONSE {store.Count}");
await Task.Delay(300);
Console.WriteLine($"AFTER_WAIT {store.Count}");
await ImportAsync("fast", 5, 80);
Console.WriteLine($"FINAL {store.Count}");
