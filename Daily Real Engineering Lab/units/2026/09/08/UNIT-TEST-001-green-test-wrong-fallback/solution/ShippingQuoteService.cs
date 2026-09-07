namespace Shipping.Core;

public interface ICarrierClient
{
    Task<decimal> GetQuoteAsync(string postalCode, CancellationToken cancellationToken = default);
}

public sealed class ShippingQuoteService(ICarrierClient carrierClient)
{
    private const decimal ApprovedFallbackQuote = 12.50m;

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
            return ApprovedFallbackQuote;
        }
    }
}
