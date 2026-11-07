record Invoice(int Id, decimal Amount);

var invoices = new[]
{
    new Invoice(1, 50m),
    new Invoice(2, 120m),
    new Invoice(3, 180m),
    new Invoice(4, 260m)
};

var maximumAmount = 150m;
var exportQuery = invoices.Where(x => x.Amount <= maximumAmount);

var expectedIdsAtReportBoundary = new[] { 1, 2 };
Console.WriteLine($"prepare: maximumAmount={maximumAmount}");

// A later phase updates a reusable option for the next report.
maximumAmount = 250m;
Console.WriteLine($"export: maximumAmount={maximumAmount}");

var actualIds = exportQuery.Select(x => x.Id).ToArray();
Console.WriteLine($"exported ids: {string.Join(',', actualIds)}");

if (args.Contains("--reproduce"))
{
    return actualIds.SequenceEqual(expectedIdsAtReportBoundary) ? 2 : 0;
}

if (args.Contains("--verify"))
{
    return actualIds.SequenceEqual(expectedIdsAtReportBoundary) ? 0 : 3;
}

return 0;