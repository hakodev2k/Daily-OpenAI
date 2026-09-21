using System.Collections.Concurrent;
using System.Diagnostics;

sealed class CatalogLoader
{
    private int _calls;
    public int Calls => _calls;

    public async Task<string> LoadAsync(string key)
    {
        Interlocked.Increment(ref _calls);
        await Task.Delay(120);
        return $"catalog:{key}:v1";
    }
}

sealed class CatalogCache(CatalogLoader loader)
{
    private readonly ConcurrentDictionary<string, string> _cache = new();

    public async Task<string> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cached))
            return cached;

        // Investigation note:
        // Observe what happens when several callers reach this path together.
        var loaded = await loader.LoadAsync(key);
        _cache[key] = loaded;
        return loaded;
    }
}

static async Task<(int LoaderCalls, long ElapsedMs, bool Correct)> RunBurstAsync(int callers)
{
    var loader = new CatalogLoader();
    var cache = new CatalogCache(loader);
    var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

    var tasks = Enumerable.Range(0, callers).Select(async _ =>
    {
        await gate.Task;
        return await cache.GetAsync("featured-products");
    }).ToArray();

    var sw = Stopwatch.StartNew();
    gate.SetResult();
    var results = await Task.WhenAll(tasks);
    sw.Stop();

    return (loader.Calls, sw.ElapsedMilliseconds,
        results.All(x => x == "catalog:featured-products:v1"));
}

var reproduce = args.Contains("--reproduce");
var verify = args.Contains("--verify");
var result = await RunBurstAsync(12);
Console.WriteLine($"callers=12 loaderCalls={result.LoaderCalls} elapsedMs={result.ElapsedMs} correct={result.Correct}");

if (reproduce)
    return result.Correct && result.LoaderCalls >= 8 ? 0 : 2;

if (verify)
    return result.Correct && result.LoaderCalls == 1 ? 0 : 3;

return 0;