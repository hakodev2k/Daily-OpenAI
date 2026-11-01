namespace ReplayLab;

public sealed record StatusSnapshot(string EventId, string SiteId, long Version, string Status);
public sealed record SiteView(long Version, string Status, string EventId);
public enum ApplyOutcome { Applied, IgnoredDuplicate, IgnoredStale, RejectedConflict }

public sealed class StatusProjection
{
    private readonly Dictionary<string, SiteView> _current = new(StringComparer.Ordinal);
    private readonly Dictionary<string, StatusSnapshot> _acceptedById = new(StringComparer.Ordinal);
    public int AppliedCount { get; private set; }

    public SiteView? Get(string siteId) =>
        _current.TryGetValue(siteId, out var view) ? view : null;

    public ApplyOutcome Apply(StatusSnapshot message)
    {
        if (message.Version <= 0)
            throw new ArgumentOutOfRangeException(nameof(message), "Version must be positive.");

        if (_acceptedById.TryGetValue(message.EventId, out var prior))
            return prior == message ? ApplyOutcome.IgnoredDuplicate : ApplyOutcome.RejectedConflict;

        if (_current.TryGetValue(message.SiteId, out var current))
        {
            if (message.Version < current.Version)
                return ApplyOutcome.IgnoredStale;
            if (message.Version == current.Version)
                return ApplyOutcome.RejectedConflict;
        }

        _current[message.SiteId] = new SiteView(message.Version, message.Status, message.EventId);
        _acceptedById.Add(message.EventId, message);
        AppliedCount++;
        return ApplyOutcome.Applied;
    }
}
