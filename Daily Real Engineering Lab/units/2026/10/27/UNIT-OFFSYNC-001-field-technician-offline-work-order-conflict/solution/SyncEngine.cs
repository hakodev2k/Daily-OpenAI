namespace OfflineSyncLab;

public enum ApplyStatus { Applied, Ignored, Conflict, AlreadyApplied }

public sealed record Change(
    string ChangeId,
    string Field,
    string Value,
    long BaseFieldRevision,
    DateTimeOffset ClientTime);

public sealed class WorkOrderSnapshot
{
    public Dictionary<string, string> Fields { get; } = new(StringComparer.Ordinal)
    {
        ["Status"] = "Assigned",
        ["TechnicianNote"] = "None"
    };

    public Dictionary<string, long> FieldRevisions { get; } = new(StringComparer.Ordinal)
    {
        ["Status"] = 0,
        ["TechnicianNote"] = 0
    };

    public long Revision { get; set; }
    public DateTimeOffset LastClientTime { get; set; } = DateTimeOffset.MinValue;
}

public sealed class SyncEngine
{
    private readonly WorkOrderSnapshot _state = new();
    private readonly Dictionary<string, Change> _acceptedChanges = new(StringComparer.Ordinal);
    public WorkOrderSnapshot State => _state;

    public ApplyStatus Apply(Change change)
    {
        if (_acceptedChanges.TryGetValue(change.ChangeId, out var accepted))
            return accepted == change ? ApplyStatus.AlreadyApplied : ApplyStatus.Conflict;

        if (!_state.Fields.ContainsKey(change.Field))
            return ApplyStatus.Conflict;

        if (change.BaseFieldRevision != _state.FieldRevisions[change.Field])
            return ApplyStatus.Conflict;

        _state.Fields[change.Field] = change.Value;
        _state.FieldRevisions[change.Field]++;
        _state.Revision++;
        _state.LastClientTime = change.ClientTime; // telemetry only, not an ordering authority
        _acceptedChanges.Add(change.ChangeId, change);
        return ApplyStatus.Applied;
    }
}
