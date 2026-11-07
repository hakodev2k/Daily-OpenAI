namespace FieldSurvey;

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
        _history.Add(new SurveyReport
        {
            SiteId = report.SiteId,
            Readings = report.Readings.ToList()
        });
    }

    public IReadOnlyList<SurveyReport> GetHistory() => _history.AsReadOnly();
}
