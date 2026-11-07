using System.Collections.Concurrent;

sealed class ReconciliationLog
{
    public ConcurrentBag<string> Entries { get; } = [];
}

sealed class ReconciliationJob(string hostName, ReconciliationLog log)
{
    private readonly SemaphoreSlim _singleRun = new(1, 1);

    public async Task RunAsync(string window)
    {
        if (!await _singleRun.WaitAsync(0)) return;
        try
        {
            Console.WriteLine($"{hostName} START {window}");
            await Task.Delay(40);
            log.Entries.Add($"{hostName}:{window}");
            Console.WriteLine($"{hostName} DONE {window}");
        }
        finally
        {
            _singleRun.Release();
        }
    }
}

var log = new ReconciliationLog();
var hostA = new ReconciliationJob("host-a", log);
var hostB = new ReconciliationJob("host-b", log);
const string firstWindow = "2026-09-21T02:00";

await Task.WhenAll(hostA.RunAsync(firstWindow), hostB.RunAsync(firstWindow));
var firstCount = log.Entries.Count(x => x.EndsWith(firstWindow));
Console.WriteLine($"window={firstWindow} businessExecutions={firstCount}");

if (args.Contains("--reproduce"))
    return firstCount == 2 ? 0 : 2;

if (args.Contains("--verify"))
{
    const string secondWindow = "2026-09-21T03:00";
    await Task.WhenAll(hostA.RunAsync(secondWindow), hostB.RunAsync(secondWindow));
    var secondCount = log.Entries.Count(x => x.EndsWith(secondWindow));
    Console.WriteLine($"window={secondWindow} businessExecutions={secondCount}");
    return firstCount == 1 && secondCount == 1 ? 0 : 3;
}

return 0;