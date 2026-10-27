using System;
using System.Threading;
using System.Threading.Tasks;

namespace RulesetLab;

internal sealed class RulesetSnapshotCache
{
    private readonly Lazy<Task<Ruleset>> _snapshot;

    public RulesetSnapshotCache(IRulesetProvider provider)
    {
        _snapshot = new Lazy<Task<Ruleset>>(
            provider.FetchAsync,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public Task<Ruleset> GetAsync() => _snapshot.Value;
}
