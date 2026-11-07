namespace FieldSurvey;

// Reference Solution: compare after attempting your own fix.
// Replace starter/SnapshotStore.cs with this file for an optional self-check.
public sealed class SurveyReading
{
    public string PlotId { get; set; } = "";
    public string Condition { get; set; } = "";
    public List<string> Notes { get; set; } = new();
}

public sealed class SurveyReport
{
    public string SiteId { get; set; } = "";
    public List<SurveyReading> Readings { get; set; } = new();
}

public sealed class SnapshotStore
{
    private readonly List<SurveyReport> _history = new();

    public void Publish(SurveyReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        _history.Add(Clone(report));
    }

    public IReadOnlyList<SurveyReport> GetHistory() =>
        _history.Select(Clone).ToArray();

    private static SurveyReport Clone(SurveyReport report) => new()
    {
        SiteId = report.SiteId,
        Readings = report.Readings.Select(reading => new SurveyReading
        {
            PlotId = reading.PlotId,
            Condition = reading.Condition,
            Notes = reading.Notes.ToList()
        }).ToList()
    };
}
