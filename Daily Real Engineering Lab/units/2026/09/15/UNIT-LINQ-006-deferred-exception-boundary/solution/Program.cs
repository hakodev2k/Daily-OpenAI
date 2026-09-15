using System.Globalization;

var service = new ImportAmountService();
var rows = new[] { "10.50", "21.25", "not-a-number", "7.00" };

try
{
    _ = service.GetAmounts(rows).ToArray();
    Console.Error.WriteLine("Expected invalid import data.");
    return 1;
}
catch (ImportDataException ex) when (ex.InnerException is FormatException)
{
    Console.WriteLine("PASS: service translated invalid input at its boundary.");
    return 0;
}

public sealed class ImportAmountService
{
    public IEnumerable<decimal> GetAmounts(IEnumerable<string> rows)
    {
        try
        {
            return rows.Select(ParseAmount).ToArray();
        }
        catch (FormatException ex)
        {
            throw new ImportDataException("Import contains an invalid amount.", ex);
        }
    }

    private static decimal ParseAmount(string raw) =>
        decimal.Parse(raw, NumberStyles.Number, CultureInfo.InvariantCulture);
}

public sealed class ImportDataException : Exception
{
    public ImportDataException(string message, Exception innerException)
        : base(message, innerException) { }
}
