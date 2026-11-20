using System.Globalization;

record TicketLine(string Sku, decimal Net, decimal Rate);
static class TaxReport
{
    public static decimal LineTax(TicketLine line) =>
        decimal.Round(line.Net * line.Rate, 2, MidpointRounding.AwayFromZero);

    public static (decimal invoice, decimal ledger) Calculate(IReadOnlyList<TicketLine> lines)
    {
        decimal invoice = lines.Sum(LineTax);
        decimal ledger = decimal.Round(lines.Sum(x => x.Net * x.Rate), 2, MidpointRounding.AwayFromZero);
        return (invoice, ledger);
    }
}

var lines = new[]
{
    new TicketLine("BALCONY-A", 0.05m, 0.10m),
    new TicketLine("BALCONY-B", 0.05m, 0.10m),
    new TicketLine("BALCONY-C", 0.05m, 0.10m)
};
var result = TaxReport.Calculate(lines);
Console.WriteLine($"InvoiceTax={result.invoice.ToString("F2", CultureInfo.InvariantCulture)}");
Console.WriteLine($"LedgerTax={result.ledger.ToString("F2", CultureInfo.InvariantCulture)}");
Console.WriteLine($"Delta={(result.ledger - result.invoice).ToString("F2", CultureInfo.InvariantCulture)}");
if (args.Contains("--verify"))
{
    var cases = new List<TicketLine[]>
    {
        lines,
        new[] { new TicketLine("A", 1.25m, .10m), new TicketLine("B", 1.25m, .10m) },
        new[] { new TicketLine("A", 0.15m, .10m), new TicketLine("B", 0.15m, .10m), new TicketLine("C", 0.15m, .10m) },
        Array.Empty<TicketLine>()
    };
    foreach (var sample in cases)
    {
        var (invoice, ledger) = TaxReport.Calculate(sample);
        if (invoice != ledger) throw new Exception($"Reconciliation failed: invoice={invoice}, ledger={ledger}");
        if (invoice != sample.Sum(TaxReport.LineTax)) throw new Exception("Invoice line policy regression");
    }
    Console.WriteLine("VERIFY_PASS");
}
