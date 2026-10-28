using System.Collections.Concurrent;
using System.Threading;

namespace DronePermits;

internal sealed record Permit(string CorridorId, string PermitNumber);

internal sealed class PermitRegistry
{
    private readonly ConcurrentDictionary<string, Lazy<Permit>> _permits = new();
    private readonly Func<string, Permit> _issueAtAuthority;

    public PermitRegistry(Func<string, Permit> issueAtAuthority)
    {
        _issueAtAuthority = issueAtAuthority;
    }

    public Permit GetOrIssue(string corridorId)
    {
        // Losing GetOrAdd factories construct only an unevaluated Lazy object.
        // Only the Lazy instance stored in the dictionary evaluates the external call.
        var entry = _permits.GetOrAdd(corridorId, key =>
            new Lazy<Permit>(() => _issueAtAuthority(key),
                LazyThreadSafetyMode.ExecutionAndPublication));
        return entry.Value;
    }
}
