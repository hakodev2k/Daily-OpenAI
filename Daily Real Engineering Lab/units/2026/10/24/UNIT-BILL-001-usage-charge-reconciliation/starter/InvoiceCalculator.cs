namespace BillingReconciliationLab;
internal static class InvoiceCalculator
{
    // Điều tra contract giữa local billing và settlement ledger.
    public static decimal Calculate(IEnumerable<decimal> rawCharges)
    {
        ArgumentNullException.ThrowIfNull(rawCharges);
        return rawCharges.Sum(x => decimal.Round(x, 2, MidpointRounding.AwayFromZero));
    }
}
