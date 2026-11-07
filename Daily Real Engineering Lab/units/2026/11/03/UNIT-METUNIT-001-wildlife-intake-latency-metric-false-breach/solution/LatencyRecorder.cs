using System.Diagnostics.Metrics;
namespace WildlifeIntake;

public sealed class IntakeLatencyRecorder : IDisposable
{
    private readonly Meter _meter = new("Wildlife.Intake", "1.0.0");
    private readonly Histogram<double> _duration;

    public IntakeLatencyRecorder()
    {
        _duration = _meter.CreateHistogram<double>(
            "intake.processing.duration", unit: "s",
            description: "Duration of intake validation");
    }

    public void Record(TimeSpan elapsed)
    {
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        _duration.Record(elapsed.TotalSeconds);
    }

    public void Dispose() => _meter.Dispose();
}
