namespace BillingReconciliationLab;
internal static class InvoiceCalculator
{
    public static decimal Calculate(IEnumerable<decimal> rawCharges)
    {
        ArgumentNullException.ThrowIfNull(rawCharges);
        var total = rawCharges.Sum();
        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
