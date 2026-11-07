record Invoice(int Id, decimal Amount);

var invoices = new[]
{
    new Invoice(1, 50m),
    new Invoice(2, 120m),
    new Invoice(3, 180m),
    new Invoice(4, 260m)
};

var maximumAmount = 150m;
var reportMaximumAmount = maximumAmount;
var exportQuery = invoices.Where(x => x.Amount <= reportMaximumAmount);

maximumAmount = 250m;
var actualIds = exportQuery.Select(x => x.Id).ToArray();
var expected = new[] { 1, 2 };
Console.WriteLine($"exported ids: {string.Join(',', actualIds)}");
return actualIds.SequenceEqual(expected) ? 0 : 1;