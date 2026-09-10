using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

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
Console.WriteLine($"mismatch=status:{mismatch.StatusCode},order:{mismatch.OrderId},receipt:{mismatch.ReceiptId},error:{mismatch.Error}");
Console.WriteLine($"providerCalls={provider.CallCount}");

public sealed record ChargeRequest(string OrderId, long AmountCents);
public sealed record ChargeResponse(int StatusCode, string OrderId, string ReceiptId, string? Error = null);
public sealed record IdempotencyEntry(string Fingerprint, ChargeResponse Response);

public sealed class PaymentService(FakePaymentProvider provider)
{
    private readonly ConcurrentDictionary<string, IdempotencyEntry> _entries = new();

    public ChargeResponse Handle(string idempotencyKey, ChargeRequest request)
    {
        var fingerprint = Fingerprint(request);

        if (_entries.TryGetValue(idempotencyKey, out var existing))
        {
            if (!string.Equals(existing.Fingerprint, fingerprint, StringComparison.Ordinal))
            {
                return new ChargeResponse(409, request.OrderId, "", "IDEMPOTENCY_KEY_REUSED_WITH_DIFFERENT_REQUEST");
            }

            return existing.Response;
        }

        var receipt = provider.Charge(request.OrderId, request.AmountCents);
        var response = new ChargeResponse(200, request.OrderId, receipt);
        _entries[idempotencyKey] = new IdempotencyEntry(fingerprint, response);
        return response;
    }

    private static string Fingerprint(ChargeRequest request)
    {
        var canonical = $"{request.OrderId}|{request.AmountCents}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
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
