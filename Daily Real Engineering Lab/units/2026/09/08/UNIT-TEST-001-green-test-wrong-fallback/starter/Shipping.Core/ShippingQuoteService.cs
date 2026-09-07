namespace Shipping.Core;

public interface ICarrierClient
{
    Task<decimal> GetQuoteAsync(string postalCode, CancellationToken cancellationToken = default);
}

public sealed class ShippingQuoteService(ICarrierClient carrierClient)
{
    public async Task<decimal> GetQuoteAsync(
        string postalCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await carrierClient.GetQuoteAsync(postalCode, cancellationToken);
        }
        catch (TimeoutException)
        {
            // Keep checkout available when the carrier is temporarily unavailable.
            return 0m;
        }
    }
}
