namespace TransitLab;
public static class CacheKeyBuilder
{
    // Cache keys are shared by all requests for a route.
    public static string Build(string route, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        return route.Trim().ToLowerInvariant();
    }
}
