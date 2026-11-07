using Microsoft.Extensions.Logging;

public sealed class PaymentWorker
{
    private readonly ILogger<PaymentWorker> _logger;

    public PaymentWorker(ILogger<PaymentWorker> logger)
    {
        _logger = logger;
    }

    public async Task ProcessAsync(string orderId)
    {
        try
        {
            await CapturePaymentAsync(orderId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Payment capture failed for {OrderId}", orderId);
        }
    }

    private static Task CapturePaymentAsync(string orderId)
    {
        throw new InvalidOperationException($"Gateway rejected payment for {orderId}.");
    }
}