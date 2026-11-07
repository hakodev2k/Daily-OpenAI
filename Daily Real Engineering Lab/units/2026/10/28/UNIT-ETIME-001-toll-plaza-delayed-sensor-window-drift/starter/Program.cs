using System.Globalization;
using GateWindowLab;

static DateTimeOffset At(string value) =>
    DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

static IReadOnlyDictionary<DateTimeOffset, int> Expected(params (string AtUtc, int Delta)[] rows) =>
    new SortedDictionary<DateTimeOffset, int>(
        rows.ToDictionary(row => At(row.AtUtc), row => row.Delta));

var cases = new[]
{
    new Scenario(
        "Peak-hour crossing",
        new GatePassage[]
        {
            new("A-01", At("2026-10-28T10:01:20Z"), At("2026-10-28T10:01:21Z"), +1),
            new("A-02", At("2026-10-28T10:04:55Z"), At("2026-10-28T10:05:07Z"), -1),
            new("A-03", At("2026-10-28T10:05:05Z"), At("2026-10-28T10:05:06Z"), +1),
            new("A-04", At("2026-10-28T10:04:59Z"), At("2026-10-28T10:05:22Z"), +1),
            new("A-05", At("2026-10-28T10:09:59Z"), At("2026-10-28T10:10:15Z"), -1),
            new("A-06", At("2026-10-28T10:10:10Z"), At("2026-10-28T10:10:12Z"), +1)
        },
        Expected(("2026-10-28T10:00:00Z", +1),
                 ("2026-10-28T10:05:00Z", 0),
                 ("2026-10-28T10:10:00Z", +1))),
    new Scenario(
        "Offset and exact boundary",
        new GatePassage[]
        {
            new("B-01", At("2026-10-28T17:04:59+07:00"), At("2026-10-28T10:05:02Z"), +1),
            new("B-02", At("2026-10-28T17:05:00+07:00"), At("2026-10-28T10:05:01Z"), -1),
            new("B-03", At("2026-10-28T10:09:59Z"), At("2026-10-28T10:10:20Z"), +1)
        },
        Expected(("2026-10-28T10:00:00Z", +1),
                 ("2026-10-28T10:05:00Z", 0)))
};

string mode = args.FirstOrDefault() ?? "--inspect";
if (mode is not ("--inspect" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --inspect | --reproduce | --verify");
    return 2;
}

int totalMismatches = 0;
int firstMismatches = 0;
bool totalsPreserved = true;
foreach (Scenario scenario in cases)
{
    var actual = WindowRollup.Calculate(scenario.Events);
    int mismatches = 0;
    Console.WriteLine("SCENARIO: " + scenario.Name);
    foreach (DateTimeOffset key in actual.Keys.Concat(scenario.Expected.Keys).Distinct().OrderBy(x => x))
    {
        bool hasActual = actual.TryGetValue(key, out int actualValue);
        bool hasExpected = scenario.Expected.TryGetValue(key, out int expectedValue);
        bool mismatch = hasActual != hasExpected || (hasActual && actualValue != expectedValue);
        if (mismatch) mismatches++;
        Console.WriteLine(
            key.ToString("HH:mm", CultureInfo.InvariantCulture) +
            " UTC | actual=" + (hasActual ? actualValue.ToString(CultureInfo.InvariantCulture) : "missing") +
            " | expected=" + (hasExpected ? expectedValue.ToString(CultureInfo.InvariantCulture) : "missing") +
            " | " + (mismatch ? "MISMATCH" : "OK"));
    }
    int actualTotal = actual.Values.Sum();
    int expectedTotal = scenario.Expected.Values.Sum();
    if (actualTotal != expectedTotal) totalsPreserved = false;
    Console.WriteLine("Total actual=" + actualTotal + "; expected=" + expectedTotal +
                      "; mismatched windows=" + mismatches);
    totalMismatches += mismatches;
    if (scenario.Name == "Peak-hour crossing") firstMismatches = mismatches;
}

if (mode == "--inspect") return 0;
if (mode == "--reproduce")
{
    bool reproduced = firstMismatches >= 2 && totalsPreserved;
    Console.WriteLine(reproduced ? "REPRODUCED: bucket drift with unchanged total" :
                                   "NOT REPRODUCED: check untouched starter and fixtures");
    return reproduced ? 0 : 1;
}
bool verified = totalMismatches == 0 && totalsPreserved;
Console.WriteLine(verified ? "VERIFIED: all window and total contracts pass" :
                             "FAILED: window reconciliation still differs");
return verified ? 0 : 1;

internal sealed record Scenario(
    string Name,
    IReadOnlyList<GatePassage> Events,
    IReadOnlyDictionary<DateTimeOffset, int> Expected);
