using System.Security.Cryptography;
using System.Text;

var secret = Encoding.UTF8.GetBytes("local-fixture-secret");
var service = new WebhookReceiver(secret);
var payload = "{\"assetId\":\"BOOK-42\",\"event\":\"Digitized\"}";
var stamp = "2026-11-03T16:00:00Z";
var signature = Convert.ToHexString(HMACSHA256.HashData(secret, Encoding.UTF8.GetBytes(stamp + "." + payload)));
var requests = new[] {
    new WebhookRequest("delivery-101", stamp, payload, signature),
    new WebhookRequest("delivery-101", stamp, payload, signature),
    new WebhookRequest("delivery-102", "2026-10-30T16:00:00Z", payload,
        Convert.ToHexString(HMACSHA256.HashData(secret, Encoding.UTF8.GetBytes("2026-10-30T16:00:00Z." + payload))))
};
foreach (var request in requests)
    Console.WriteLine(service.Accept(request));
Console.WriteLine($"Applied={service.Applied}");

record WebhookRequest(string DeliveryId, string Timestamp, string Body, string Signature);

sealed class WebhookReceiver(byte[] secret)
{
    public int Applied { get; private set; }
    public string Accept(WebhookRequest request)
    {
        var message = Encoding.UTF8.GetBytes(request.Timestamp + "." + request.Body);
        var expected = HMACSHA256.HashData(secret, message);
        if (!Convert.TryFromHexString(request.Signature, new byte[32], out var written) || written != 32)
            return "REJECT signature";
        var supplied = Convert.FromHexString(request.Signature);
        if (!CryptographicOperations.FixedTimeEquals(expected, supplied))
            return "REJECT signature";
        Applied++;
        return "ACCEPT";
    }
}
