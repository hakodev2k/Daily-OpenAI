using System.Collections.Concurrent;

var mode = args.Any(a => string.Equals(a, "--verify", StringComparison.OrdinalIgnoreCase)) ? "verify" : "reproduce";
var cache = new FlakyCache();
var origin = new OriginStore();
var service = new ContentService(cache, origin, message => Console.WriteLine($"LOG {message}"));
var failures = new List<string>();
var successfulRequests = 0;

for (var request = 1; request <= 8; request++)
{
    cache.IsAvailable = request is not 4 and not 5;
    try
    {
        var value = await service.GetAsync("home", CancellationToken.None);
        if (value != "home:v42") failures.Add($"request={request} wrong-value={value}");
        else successfulRequests++;
    }
    catch (Exception ex)
    {
        failures.Add($"request={request} {ex.GetType().Name}: {ex.Message}");
    }
}

cache.IsAvailable = true;
var beforeProbe = origin.CallCount;
var recoveredValue = await service.GetAsync("home", CancellationToken.None);
var recoveredFromCache = recoveredValue == "home:v42" && origin.CallCount == beforeProbe;

Console.WriteLine($"MODE={mode}");
Console.WriteLine($"successfulRequests={successfulRequests}");
Console.WriteLine($"failures={failures.Count}");
Console.WriteLine($"originCalls={origin.CallCount}");
Console.WriteLine($"cacheReadFailures={cache.ReadFailures}");
Console.WriteLine($"cacheWriteFailures={cache.WriteFailures}");
Console.WriteLine($"recoveredFromCache={recoveredFromCache}");
foreach (var failure in failures) Console.WriteLine($"FAIL {failure}");

if (mode == "reproduce")
{
    if (failures.Count == 2 && cache.ReadFailures == 2)
    {
        Console.WriteLine("SYMPTOM_CONFIRMED");
        Environment.ExitCode = 1;
        return;
    }
    Console.WriteLine("REPRODUCTION_NOT_OBSERVED_AS_EXPECTED");
    Environment.ExitCode = 2;
    return;
}

var passed = failures.Count == 0 && successfulRequests == 8 && origin.CallCount is >= 3 and <= 4 && recoveredFromCache;
Console.WriteLine(passed ? "VERIFICATION_PASSED" : "VERIFICATION_FAILED");
Environment.ExitCode = passed ? 0 : 1;

interface IContentCache
{
    Task<string?> GetAsync(string key, CancellationToken cancellationToken);
    Task SetAsync(string key, string value, CancellationToken cancellationToken);
}
sealed class FlakyCache : IContentCache
{
    private readonly ConcurrentDictionary<string,string> _items = new();
    public bool IsAvailable { get; set; } = true;
    public int ReadFailures { get; private set; }
    public int WriteFailures { get; private set; }
    public Task<string?> GetAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsAvailable) { ReadFailures++; throw new TimeoutException("Simulated Redis read timeout."); }
        _items.TryGetValue(key, out var value);
        return Task.FromResult<string?>(value);
    }
    public Task SetAsync(string key, string value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsAvailable) { WriteFailures++; throw new TimeoutException("Simulated Redis write timeout."); }
        _items[key] = value;
        return Task.CompletedTask;
    }
}
sealed class OriginStore
{
    public int CallCount { get; private set; }
    public async Task<string> LoadAsync(string key, CancellationToken cancellationToken)
    {
        CallCount++;
        await Task.Delay(15, cancellationToken);
        return $"{key}:v42";
    }
}
sealed class ContentService
{
    private readonly IContentCache _cache; private readonly OriginStore _origin; private readonly Action<string> _log;
    public ContentService(IContentCache cache, OriginStore origin, Action<string> log) { _cache=cache; _origin=origin; _log=log; }
    public async Task<string> GetAsync(string key, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync(key, cancellationToken);
        if (cached is not null) { _log($"cache-hit key={key}"); return cached; }
        _log($"cache-miss key={key}");
        var fresh = await _origin.LoadAsync(key, cancellationToken);
        await _cache.SetAsync(key, fresh, cancellationToken);
        return fresh;
    }
}
