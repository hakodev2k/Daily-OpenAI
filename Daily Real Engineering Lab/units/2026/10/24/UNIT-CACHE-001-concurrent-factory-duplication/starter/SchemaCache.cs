using System.Collections.Concurrent;
namespace SchemaFactoryLab;
public sealed record CompiledSchema(string Key, Guid ArtifactId);
public sealed class SchemaCache
{
    private readonly ConcurrentDictionary<string, CompiledSchema> _cache = new(StringComparer.Ordinal);
    private readonly Func<string, CompiledSchema> _compiler;
    public SchemaCache(Func<string, CompiledSchema> compiler) => _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    public CompiledSchema Get(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _cache.GetOrAdd(key, _compiler);
    }
}
