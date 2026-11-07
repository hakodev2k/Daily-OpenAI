var edge = new EdgeCache();

Run("warm-en", edge, "/article/42", "en-US", "EN:42");
Run("then-vi", edge, "/article/42", "vi-VN", "VI:42");
Run("repeat-en", edge, "/article/42", "en-US", "EN:42");
edge.Clear();
Run("warm-vi", edge, "/article/42", "vi-VN", "VI:42");
Run("then-en", edge, "/article/42", "en-US", "EN:42");
Run("repeat-vi", edge, "/article/42", "vi-VN", "VI:42");

static void Run(string name, EdgeCache edge, string path, string language, string expected)
{
    var result = edge.Get(path, language);
    Console.WriteLine($"{name}: request={language} source={result.Source} body={result.Body} expected={expected} match={result.Body == expected}");
}

sealed class EdgeCache
{
    private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);

    public (string Body, string Source) Get(string path, string acceptLanguage)
    {
        // Investigation note: Which request properties can change the representation?
        var cacheKey = path;
        if (_cache.TryGetValue(cacheKey, out var cached)) return (cached, "EDGE-HIT");

        var body = Origin(path, acceptLanguage);
        _cache[cacheKey] = body;
        return (body, "ORIGIN");
    }

    public void Clear() => _cache.Clear();

    private static string Origin(string path, string acceptLanguage)
    {
        var id = path.Split('/').Last();
        return acceptLanguage.StartsWith("vi", StringComparison.OrdinalIgnoreCase) ? $"VI:{id}" : $"EN:{id}";
    }
}
