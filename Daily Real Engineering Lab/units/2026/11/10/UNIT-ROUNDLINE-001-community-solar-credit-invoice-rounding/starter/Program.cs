using System.Globalization;

record CreditLine(string Id, decimal Quantity, decimal UnitPrice);

static class Billing
{
    public static decimal InvoiceTotal(IEnumerable<CreditLine> lines)
    {
        decimal rawTotal = 0m;
        foreach (var line in lines)
            rawTotal += line.Quantity * line.UnitPrice;
        return Math.Round(rawTotal, 2);
    }
}

var cases = new (string Name, CreditLine[] Lines, decimal Expected)[]
{
    ("mixed-credit", [new("A",1m,0.005m),new("B",1m,0.005m),new("C",1m,-0.005m)], 0.01m),
    ("positive-lines", [new("D",1m,0.015m),new("E",1m,0.015m)], 0.04m),
    ("negative-lines", [new("F",1m,-0.015m),new("G",1m,-0.015m)], -0.04m)
};
int mismatches = 0;
foreach (var item in cases)
{
    var actual = Billing.InvoiceTotal(item.Lines);
    Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{item.Name}: actual={actual:F2} expected={item.Expected:F2}"));
    if (actual != item.Expected) mismatches++;
}
Console.WriteLine($"MISMATCHES={mismatches}");
if (args.Contains("--assert-contract") && mismatches != 0) Environment.ExitCode = 1;
