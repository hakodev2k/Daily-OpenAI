using System.Diagnostics.Metrics;
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

var sourceMs = new[] { 120, 180, 240, 300, 600 };
using (var recorder = new IntakeLatencyRecorder())
    foreach (var ms in sourceMs)
        recorder.Record(TimeSpan.FromMilliseconds(ms));

Require(unit == "s", "Unit contract must be seconds.");
Require(samples.Count == sourceMs.Length, "Exactly one sample per request.");
for (var i = 0; i < sourceMs.Length; i++)
{
    Require(double.IsFinite(samples[i]), "Non-finite measurement.");
    Require(Math.Abs(samples[i] - sourceMs[i] / 1000.0) < 1e-9,
        "Measurement differs from independent request trace.");
}
samples.Sort();
var p95 = samples[(int)Math.Ceiling(samples.Count * 0.95) - 1];
Require(Math.Abs(p95 - 0.6) < 1e-9, "Incorrect p95.");
Require(p95 <= 0.75, "False SLO breach.");
Console.WriteLine("PASS: learner starter emits correct unit, values and SLO decision.");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException("FAIL: " + message);
}
