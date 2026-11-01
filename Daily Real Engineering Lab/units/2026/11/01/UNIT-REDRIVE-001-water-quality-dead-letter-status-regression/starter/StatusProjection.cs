namespace ReplayLab;

public sealed record StatusSnapshot(string EventId, string SiteId, long Version, string Status);
public sealed record SiteView(long Version, string Status, string EventId);
public enum ApplyOutcome { Applied, IgnoredDuplicate, IgnoredStale, RejectedConflict }

public sealed class StatusProjection
{
    private readonly Dictionary<string, SiteView> _current = new(StringComparer.Ordinal);
    private readonly HashSet<string> _handledIds = new(StringComparer.Ordinal);
    public int AppliedCount { get; private set; }

    public SiteView? Get(string siteId) =>
        _current.TryGetValue(siteId, out var view) ? view : null;

    public ApplyOutcome Apply(StatusSnapshot message)
    {
        if (!_handledIds.Add(message.EventId))
            return ApplyOutcome.IgnoredDuplicate;

        // The read model is refreshed whenever a broker delivery is handled.
        _current[message.SiteId] = new SiteView(message.Version, message.Status, message.EventId);
        AppliedCount++;
        return ApplyOutcome.Applied;
    }
}
