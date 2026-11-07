using Shipping.Core;

var service = new ShippingQuoteService(new TimeoutCarrierClient());
var quote = await service.GetQuoteAsync("10001");

Console.WriteLine($"FallbackQuote={quote:0.00}");

file sealed class TimeoutCarrierClient : ICarrierClient
{
    public Task<decimal> GetQuoteAsync(
        string postalCode,
        CancellationToken cancellationToken = default)
        => throw new TimeoutException("Carrier did not respond in time.");
}
