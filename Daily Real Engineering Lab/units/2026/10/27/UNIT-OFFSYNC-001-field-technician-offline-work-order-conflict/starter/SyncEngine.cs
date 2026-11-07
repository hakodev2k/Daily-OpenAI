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
    public WorkOrderSnapshot State => _state;

    public ApplyStatus Apply(Change change)
    {
        if (!_state.Fields.ContainsKey(change.Field))
            return ApplyStatus.Conflict;

        // Existing field gateway behavior; investigate its contract.
        if (change.ClientTime < _state.LastClientTime)
            return ApplyStatus.Ignored;

        _state.Fields[change.Field] = change.Value;
        _state.FieldRevisions[change.Field]++;
        _state.Revision++;
        _state.LastClientTime = change.ClientTime;
        return ApplyStatus.Applied;
    }
}
