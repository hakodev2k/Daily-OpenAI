namespace FieldOpsLab;

internal sealed class DailyRollup
{
    private readonly Dictionary<string, WorkSummary> _byTechnician = new(StringComparer.Ordinal);

    public void Add(WorkEvent entry)
    {
        if (string.IsNullOrWhiteSpace(entry.TechnicianId))
            throw new ArgumentException("TechnicianId must be provided.", nameof(entry));

        if (!_byTechnician.TryGetValue(entry.TechnicianId, out var current))
            current = new WorkSummary();

        current.Record(entry.Succeeded);

        // Investigation note: compare the state observed here with a later Read().
    }

    public WorkSummary Read(string technicianId) =>
        _byTechnician.TryGetValue(technicianId, out var summary) ? summary : default;
}
