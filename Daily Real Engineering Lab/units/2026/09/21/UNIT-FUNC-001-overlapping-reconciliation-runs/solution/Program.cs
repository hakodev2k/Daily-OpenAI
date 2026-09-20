using System.Collections.Concurrent;

sealed class LeaseStore
{
    private readonly ConcurrentDictionary<string, string> _owners = new();
    public bool TryAcquire(string key, string owner) => _owners.TryAdd(key, owner);
}

sealed class ReconciliationLog { public ConcurrentBag<string> Entries { get; } = []; }

sealed class ReconciliationJob(string host, LeaseStore leases, ReconciliationLog log)
{
    public async Task RunAsync(string window)
    {
        if (!leases.TryAcquire(window, host)) return;
        await Task.Delay(40);
        log.Entries.Add($"{host}:{window}");
    }
}

var leases = new LeaseStore();
var log = new ReconciliationLog();
var a = new ReconciliationJob("host-a", leases, log);
var b = new ReconciliationJob("host-b", leases, log);
const string window = "2026-09-21T02:00";
await Task.WhenAll(a.RunAsync(window), b.RunAsync(window));
Console.WriteLine($"businessExecutions={log.Entries.Count}");
return log.Entries.Count == 1 ? 0 : 1;