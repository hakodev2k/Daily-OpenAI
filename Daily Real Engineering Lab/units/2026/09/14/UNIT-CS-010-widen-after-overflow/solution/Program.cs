const int quantity = 70_000;
const int unitPriceCents = 70_000;

var calculator = new InvoiceCalculator();
Console.WriteLine(calculator.CalculateTotalCents(quantity, unitPriceCents));

internal sealed class InvoiceCalculator
{
    public long CalculateTotalCents(int quantity, int unitPriceCents)
        => checked((long)quantity * unitPriceCents);
}
