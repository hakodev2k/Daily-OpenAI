using System;
using System.Threading.Tasks;

namespace RulesetLab;

internal sealed class RulesetSnapshotCache
{
    private readonly IRulesetProvider _provider;
    private readonly object _gate = new();
    private Task<Ruleset>? _current;

    public RulesetSnapshotCache(IRulesetProvider provider) => _provider = provider;

    public Task<Ruleset> GetAsync()
    {
        lock (_gate)
        {
            if (_current is null || _current.IsFaulted || _current.IsCanceled)
                _current = _provider.FetchAsync();

            return _current;
        }
    }
}
