namespace TransitLab;
public static class CacheKeyBuilder
{
    public static string Build(string route, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        return $"{route.Trim().ToLowerInvariant().Length}:{route.Trim().ToLowerInvariant()}|{language.Trim().ToLowerInvariant()}";
    }
}
