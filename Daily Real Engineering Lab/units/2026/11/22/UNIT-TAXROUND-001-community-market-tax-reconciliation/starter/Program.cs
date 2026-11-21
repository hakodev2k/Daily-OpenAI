using System.Globalization;
namespace MarketTaxLab;
public record SaleLine(string Sku, decimal Net, decimal Rate);
public static class TaxCalculator
{
    public static decimal Tax(IReadOnlyList<SaleLine> lines)
    {
        // Existing reporting implementation: group a basket before producing the tax amount.
        return decimal.Round(lines.Sum(x => x.Net * x.Rate), 2, MidpointRounding.AwayFromZero);
    }
}
public static class Program
{
    static readonly SaleLine[] Basket = [
        new("A", 0.05m, 0.10m), new("B", 0.05m, 0.10m), new("C", 0.05m, 0.10m)
    ];
    public static int Main(string[] args)
    {
        decimal actual = TaxCalculator.Tax(Basket);
        Console.WriteLine($"Basket tax: {actual.ToString("F2", CultureInfo.InvariantCulture)}");
        if (!args.Contains("--verify")) return 0;
        var checks = new (string Name, SaleLine[] Lines, decimal Expected)[] {
            ("three-small-lines", Basket, 0.03m),
            ("single-midpoint", [new("D", 0.05m, 0.10m)], 0.01m),
            ("mixed-rates", [new("E", 0.05m, 0.10m), new("F", 0.05m, 0.20m)], 0.02m),
            ("refund-symmetry", [new("G", -0.05m, 0.10m), new("H", -0.05m, 0.10m)], -0.02m),
            ("empty-basket", [], 0m)
        };
        int failed = 0;
        foreach (var check in checks) {
            var got = TaxCalculator.Tax(check.Lines);
            Console.WriteLine($"{check.Name}: expected={check.Expected:F2} actual={got:F2} {(got == check.Expected ? "PASS" : "FAIL")}");
            if (got != check.Expected) failed++;
        }
        return failed == 0 ? 0 : 1;
    }
}
