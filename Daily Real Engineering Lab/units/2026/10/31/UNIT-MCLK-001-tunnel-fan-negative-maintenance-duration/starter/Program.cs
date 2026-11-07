using ClockLab;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
var cases = new[]
{
    new Scenario("stable", TimeSpan.FromMilliseconds(250), TimeSpan.Zero),
    new Scenario("backward-step", TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(-30)),
    new Scenario("forward-step", TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(45))
};
var results = new List<(Scenario Scenario, MaintenanceMeasurement Result)>();

foreach (var scenario in cases)
{
    var clock = new SimulatedTimeProvider(
        new DateTimeOffset(2026, 10, 31, 8, 0, 0, TimeSpan.Zero));
    var measurement = await MaintenanceProbe.MeasureAsync(clock, () =>
    {
        clock.Advance(scenario.Elapsed, scenario.ClockAdjustment);
        return Task.CompletedTask;
    });
    results.Add((scenario, measurement));
    Console.WriteLine(
        $"{scenario.Name,-13} DurationMs={measurement.Duration.TotalMilliseconds,9:0} " +
        $"StartedAtUtc={measurement.StartedAtUtc:O} FinishedAtUtc={measurement.FinishedAtUtc:O}");
}

var durationsCorrect = results.All(x => x.Result.Duration == x.Scenario.Elapsed);
var auditCorrect = results.All(x =>
    x.Result.FinishedAtUtc - x.Result.StartedAtUtc ==
    x.Scenario.Elapsed + x.Scenario.ClockAdjustment);

if (mode == "reproduce")
{
    var originalSymptom =
        results[0].Result.Duration == cases[0].Elapsed &&
        results[1].Result.Duration != cases[1].Elapsed &&
        results[2].Result.Duration != cases[2].Elapsed;
    Console.WriteLine(originalSymptom
        ? "REPRODUCED: baseline symptom confirmed; this is not a fix."
        : "NOT REPRODUCED: original symptom absent.");
    return originalSymptom ? 0 : 1;
}
if (mode == "verify")
{
    Console.WriteLine(
        $"VERIFY: elapsed-duration={(durationsCorrect ? "PASS" : "FAIL")}, " +
        $"audit-timestamps={(auditCorrect ? "PASS" : "FAIL")}");
    return durationsCorrect && auditCorrect ? 0 : 1;
}
if (mode != "demo")
{
    Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
    return 2;
}
return 0;

internal sealed record Scenario(
    string Name, TimeSpan Elapsed, TimeSpan ClockAdjustment);
