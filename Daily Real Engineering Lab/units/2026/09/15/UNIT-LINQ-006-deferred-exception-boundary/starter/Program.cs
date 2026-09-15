using System.Globalization;

var mode = args.FirstOrDefault() ?? "run";
var service = new ImportAmountService();
var rows = new[] { "10.50", "21.25", "not-a-number", "7.00" };

if (mode.Equals("reproduce", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var amounts = service.GetAmounts(rows);
        Console.WriteLine("Service returned. Enumeration starts now.");
        foreach (var amount in amounts)
        {
            Console.WriteLine($"Amount={amount:F2}");
        }

        Console.Error.WriteLine("Expected the starter symptom, but enumeration completed.");
        return 2;
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Observed raw exception outside service boundary: {ex.GetType().Name}");
        return 0;
    }
    catch (ImportDataException ex)
    {
        Console.Error.WriteLine($"The learner fix appears active: {ex.Message}");
        return 3;
    }
}

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var amounts = service.GetAmounts(rows);
        _ = amounts.ToArray();
        Console.Error.WriteLine("Expected ImportDataException for invalid import data.");
        return 4;
    }
    catch (ImportDataException ex) when (ex.InnerException is FormatException)
    {
        Console.WriteLine("PASS: invalid data is translated at the service boundary.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"FAIL: unexpected exception escaped: {ex.GetType().Name}");
        return 5;
    }
}

Console.WriteLine("Use 'reproduce' or 'verify'.");
return 0;

public sealed class ImportAmountService
{
    public IEnumerable<decimal> GetAmounts(IEnumerable<string> rows)
    {
        try
        {
            return rows.Select(ParseAmount);
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
        : base(message, innerException)
    {
    }
}
