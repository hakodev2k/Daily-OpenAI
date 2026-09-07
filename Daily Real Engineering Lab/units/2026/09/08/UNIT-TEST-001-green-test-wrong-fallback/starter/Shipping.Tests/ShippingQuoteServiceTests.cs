using Shipping.Core;

namespace Shipping.Tests;

public sealed class ShippingQuoteServiceTests
{
    [Fact]
    public async Task Carrier_timeout_does_not_crash_checkout_and_calls_carrier_once()
    {
        var carrier = new TimeoutCarrierClient();
        var service = new ShippingQuoteService(carrier);

        var exception = await Record.ExceptionAsync(
            () => service.GetQuoteAsync("10001"));

        Assert.Null(exception);
        Assert.Equal(1, carrier.CallCount);
    }

    private sealed class TimeoutCarrierClient : ICarrierClient
    {
        public int CallCount { get; private set; }

        public Task<decimal> GetQuoteAsync(
            string postalCode,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            throw new TimeoutException("Carrier did not respond in time.");
        }
    }
}
