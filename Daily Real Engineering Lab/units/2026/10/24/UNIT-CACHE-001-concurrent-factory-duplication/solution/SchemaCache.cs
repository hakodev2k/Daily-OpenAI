using System.Collections.Concurrent;
using System.Threading;
namespace SchemaFactoryLab;
public sealed record CompiledSchema(string Key, Guid ArtifactId);
public sealed class SchemaCache
{
    private readonly ConcurrentDictionary<string, Lazy<CompiledSchema>> _cache = new(StringComparer.Ordinal);
    private readonly Func<string, CompiledSchema> _compiler;
    public SchemaCache(Func<string, CompiledSchema> compiler) => _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    public CompiledSchema Get(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _cache.GetOrAdd(key, k => new Lazy<CompiledSchema>(
            () => _compiler(k), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }
}
