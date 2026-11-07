const int quantity = 70_000;
const int unitPriceCents = 70_000;
const long expectedTotalCents = 4_900_000_000L;

var calculator = new InvoiceCalculator();
var actual = calculator.CalculateTotalCents(quantity, unitPriceCents);

Console.WriteLine($"quantity={quantity}");
Console.WriteLine($"unitPriceCents={unitPriceCents}");
Console.WriteLine($"totalCents={actual}");
Console.WriteLine($"expectedTotalCents={expectedTotalCents}");

if (args.Contains("--assert-fixed") && actual != expectedTotalCents)
    Environment.Exit(1);

internal sealed class InvoiceCalculator
{
    public long CalculateTotalCents(int quantity, int unitPriceCents)
    {
        // Investigation note:
        // The return type is intentionally wider than each input.
        // Determine the type used to evaluate the arithmetic expression itself.
        return quantity * unitPriceCents;
    }
}
