using System.Globalization;
var rides = new[] { new Ride("R-101", 1.005m), new Ride("R-102", 2.015m), new Ride("R-103", 3.125m), new Ride("R-104", 4.234m) };
var charges = rides.Select(r => (r.Id, Amount: Settlement.Charge(r.Fee))).ToArray();
foreach (var c in charges) Console.WriteLine($"{c.Id}: {c.Amount.ToString("F2", CultureInfo.InvariantCulture)}");
Console.WriteLine($"TOTAL: {charges.Sum(c => c.Amount).ToString("F2", CultureInfo.InvariantCulture)}");
if (args.Contains("--verify")) {
    decimal[] expected = [1.01m, 2.02m, 3.13m, 4.23m];
    if (!charges.Select(c => c.Amount).SequenceEqual(expected) || charges.Sum(c => c.Amount) != 10.39m) {
        Console.Error.WriteLine("FAIL: per-ride partner statement mismatch"); Environment.ExitCode = 1;
    } else Console.WriteLine("PASS: partner settlement contract");
}
record Ride(string Id, decimal Fee);
static class Settlement {
    public static decimal Charge(decimal fee) {
        if (fee < 0) throw new ArgumentOutOfRangeException(nameof(fee));
        return Math.Round(fee, 2, MidpointRounding.AwayFromZero);
    }
}
