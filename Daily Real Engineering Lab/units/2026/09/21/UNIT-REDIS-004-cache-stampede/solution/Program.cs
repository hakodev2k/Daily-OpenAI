using System.Collections.Concurrent;

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
    private readonly ConcurrentDictionary<string, Lazy<Task<string>>> _inflight = new();

    public async Task<string> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cached)) return cached;

        var lazy = _inflight.GetOrAdd(key, k =>
            new Lazy<Task<string>>(() => LoadAndCacheAsync(k), LazyThreadSafetyMode.ExecutionAndPublication));

        try { return await lazy.Value; }
        finally { _inflight.TryRemove(new KeyValuePair<string, Lazy<Task<string>>>(key, lazy)); }
    }

    private async Task<string> LoadAndCacheAsync(string key)
    {
        var value = await loader.LoadAsync(key);
        _cache[key] = value;
        return value;
    }
}