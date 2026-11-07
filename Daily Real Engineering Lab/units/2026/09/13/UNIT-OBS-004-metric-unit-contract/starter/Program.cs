using System.Diagnostics.Metrics;

const string MeterName = "Checkout.ObservabilityLab";
var captured = new List<double>();

using var listener = new MeterListener();
listener.InstrumentPublished = (instrument, currentListener) =>
{
    if (instrument.Meter.Name == MeterName)
    {
        currentListener.EnableMeasurementEvents(instrument);
    }
};
listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
{
    captured.Add(measurement);
    Console.WriteLine($"{instrument.Name} [{instrument.Unit}] = {measurement:F3}");
});
listener.Start();

using var meter = new Meter(MeterName, "1.0.0");
var dependencyDuration = meter.CreateHistogram<double>(
    "checkout.dependency.duration",
    unit: "ms",
    description: "Observed downstream dependency duration");

RecordInventory(dependencyDuration, TimeSpan.FromMilliseconds(78));
RecordInventory(dependencyDuration, TimeSpan.FromMilliseconds(92));
RecordPricing(dependencyDuration, TimeSpan.FromMilliseconds(110));
RecordPricing(dependencyDuration, TimeSpan.FromMilliseconds(126));

listener.RecordObservableInstruments();

Console.WriteLine();
Console.WriteLine($"Measurements captured: {captured.Count}");

var contractSatisfied = captured.Count == 4 && captured.All(value => value is >= 10 and <= 1000);
Console.WriteLine(contractSatisfied
    ? "Telemetry contract: PASS"
    : "Telemetry contract: FAIL");

return contractSatisfied ? 0 : 1;

static void RecordInventory(Histogram<double> duration, TimeSpan elapsed)
{
    duration.Record(elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("dependency", "inventory"));
}

static void RecordPricing(Histogram<double> duration, TimeSpan elapsed)
{
    // Investigation note: this code path was added by another team.
    // Compare the meaning of the recorded value with the instrument contract.
    duration.Record(elapsed.TotalSeconds, new KeyValuePair<string, object?>("dependency", "pricing"));
}
