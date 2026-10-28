using System.Collections.Concurrent;

namespace DronePermits;

internal sealed record Permit(string CorridorId, string PermitNumber);

internal sealed class PermitRegistry
{
    private readonly ConcurrentDictionary<string, Permit> _permits = new();
    private readonly Func<string, Permit> _issueAtAuthority;

    public PermitRegistry(Func<string, Permit> issueAtAuthority)
    {
        _issueAtAuthority = issueAtAuthority;
    }

    public Permit GetOrIssue(string corridorId)
    {
        // This cache is shared by all concurrent request handlers.
        return _permits.GetOrAdd(corridorId, key => _issueAtAuthority(key));
    }
}
