namespace FieldOpsLab;

internal sealed class DailyRollup
{
    private readonly Dictionary<string, WorkSummary> _byTechnician = new(StringComparer.Ordinal);

    public void Add(WorkEvent entry)
    {
        if (string.IsNullOrWhiteSpace(entry.TechnicianId))
            throw new ArgumentException("TechnicianId must be provided.", nameof(entry));

        _byTechnician.TryGetValue(entry.TechnicianId, out var current);
        current.Record(entry.Succeeded);
        _byTechnician[entry.TechnicianId] = current;
    }

    public WorkSummary Read(string technicianId) =>
        _byTechnician.TryGetValue(technicianId, out var summary) ? summary : default;
}
