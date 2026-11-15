using System.Globalization;

static decimal Payout(decimal amount, decimal rate)
{
    decimal raw = amount * rate;
    return Math.Round(raw, 2);
}

var cases = new (string Student, decimal Base, decimal Rate, decimal Expected)[]
{
    ("SV101", 1.05m, 0.5m, 0.53m),
    ("SV102", 1.25m, 0.5m, 0.63m),
    ("SV103", 1.15m, 0.5m, 0.58m),
    ("SV104", 2.00m, 0.5m, 1.00m)
};
decimal actualTotal = 0, expectedTotal = 0;
int mismatches = 0;
foreach (var row in cases)
{
    var actual = Payout(row.Base, row.Rate);
    actualTotal += actual;
    expectedTotal += row.Expected;
    if (actual != row.Expected) mismatches++;
    Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{row.Student} actual={actual:F2} expected={row.Expected:F2} match={actual == row.Expected}"));
}
Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"total={actualTotal:F2} expectedTotal={expectedTotal:F2} mismatches={mismatches}"));
if (args.Contains("--verify", StringComparer.OrdinalIgnoreCase) && mismatches != 0) Environment.ExitCode = 1;
if (args.Contains("--reproduce", StringComparer.OrdinalIgnoreCase) && mismatches == 0) Environment.ExitCode = 1;
