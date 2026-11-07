public readonly record struct RosterSnapshot(int ActiveCount, long Revision);

public sealed class ShiftRosterStore
{
    private readonly object _sync = new();
    private readonly Dictionary<string, bool> _onDuty;
    private long _revision;

    public ShiftRosterStore(IEnumerable<string> staffIds)
    {
        _onDuty = staffIds.ToDictionary(id => id, _ => true, StringComparer.Ordinal);
    }

    public RosterSnapshot ReadSnapshot()
    {
        lock (_sync)
        {
            return new RosterSnapshot(_onDuty.Values.Count(active => active), _revision);
        }
    }

    public bool MarkOffDuty(string staffId)
    {
        lock (_sync)
        {
            if (!_onDuty.TryGetValue(staffId, out var active) || !active) return false;
            _onDuty[staffId] = false;
            _revision++;
            return true;
        }
    }

    // Mô phỏng một atomic database operation: check + update cùng một critical section.
    public bool TryApproveLeaveKeepingCoverage(string staffId, int minimumOnDuty)
    {
        if (minimumOnDuty < 0) throw new ArgumentOutOfRangeException(nameof(minimumOnDuty));
        lock (_sync)
        {
            if (!_onDuty.TryGetValue(staffId, out var active) || !active) return false;
            if (_onDuty.Values.Count(isOnDuty => isOnDuty) <= minimumOnDuty) return false;
            _onDuty[staffId] = false;
            _revision++;
            return true;
        }
    }

    public int ActiveCount
    {
        get { lock (_sync) return _onDuty.Values.Count(active => active); }
    }
}
