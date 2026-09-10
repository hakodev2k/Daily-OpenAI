using System.Collections.Concurrent;

var provider = new FakePaymentProvider();
var service = new PaymentService(provider);

var firstRequest = new ChargeRequest("ORDER-100", 125_00);
var retryRequest = new ChargeRequest("ORDER-100", 125_00);
var differentRequest = new ChargeRequest("ORDER-200", 80_00);
const string key = "mobile-req-42";

var first = service.Handle(key, firstRequest);
var retry = service.Handle(key, retryRequest);
var mismatch = service.Handle(key, differentRequest);

Console.WriteLine($"first=status:{first.StatusCode},order:{first.OrderId},receipt:{first.ReceiptId}");
Console.WriteLine($"retry=status:{retry.StatusCode},order:{retry.OrderId},receipt:{retry.ReceiptId}");
Console.WriteLine($"mismatch=status:{mismatch.StatusCode},order:{mismatch.OrderId},receipt:{mismatch.ReceiptId}");
Console.WriteLine($"providerCalls={provider.CallCount}");

public sealed record ChargeRequest(string OrderId, long AmountCents);
public sealed record ChargeResponse(int StatusCode, string OrderId, string ReceiptId, string? Error = null);

public sealed class PaymentService(FakePaymentProvider provider)
{
    private readonly ConcurrentDictionary<string, ChargeResponse> _responses = new();

    public ChargeResponse Handle(string idempotencyKey, ChargeRequest request)
    {
        if (_responses.TryGetValue(idempotencyKey, out var existing))
        {
            return existing;
        }

        var receipt = provider.Charge(request.OrderId, request.AmountCents);
        var response = new ChargeResponse(200, request.OrderId, receipt);
        _responses[idempotencyKey] = response;
        return response;
    }
}

public sealed class FakePaymentProvider
{
    public int CallCount { get; private set; }

    public string Charge(string orderId, long amountCents)
    {
        CallCount++;
        return $"rcpt-{CallCount:D3}";
    }
}
