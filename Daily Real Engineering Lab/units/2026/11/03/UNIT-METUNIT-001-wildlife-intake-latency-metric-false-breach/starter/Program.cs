using System.Diagnostics.Metrics;
using System.Globalization;
using WildlifeIntake;

var samples = new List<double>();
string? unit = null;
using var listener = new MeterListener();
listener.InstrumentPublished = (instrument, active) =>
{
    if (instrument.Meter.Name == "Wildlife.Intake" &&
        instrument.Name == "intake.processing.duration")
    {
        unit = instrument.Unit;
        active.EnableMeasurementEvents(instrument);
    }
};
listener.SetMeasurementEventCallback<double>((instrument, value, tags, state) =>
    samples.Add(value));
listener.Start();

var durationsMs = new[] { 120, 180, 240, 300, 600 };
using (var recorder = new IntakeLatencyRecorder())
    foreach (var ms in durationsMs)
        recorder.Record(TimeSpan.FromMilliseconds(ms));

if (samples.Count != durationsMs.Length)
    throw new InvalidOperationException("Missing measurements.");

samples.Sort();
var p95 = samples[(int)Math.Ceiling(samples.Count * 0.95) - 1];
const double thresholdSeconds = 0.75;
Console.WriteLine("UNIT=" + (unit ?? "<missing>"));
Console.WriteLine("SAMPLES=" + string.Join(",", samples.Select(x =>
    x.ToString("0.000", CultureInfo.InvariantCulture))));
Console.WriteLine("P95_SECONDS=" + p95.ToString("0.000", CultureInfo.InvariantCulture));
Console.WriteLine("SLO_THRESHOLD_SECONDS=0.750");
Console.WriteLine("BREACH=" + (p95 > thresholdSeconds ? "true" : "false"));
