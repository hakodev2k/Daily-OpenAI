using System.Globalization;
using System.Security.Cryptography;
using System.Text;

var secret = Encoding.UTF8.GetBytes("local-fixture-secret");
var clock = new DateTimeOffset(2026, 11, 3, 16, 9, 21, TimeSpan.Zero);
var receiver = new WebhookReceiver(secret, () => clock);
var body = "{\"assetId\":\"BOOK-42\",\"event\":\"Digitized\"}";
WebhookRequest Make(string id, string stamp) => new(id, stamp, body,
    Convert.ToHexString(HMACSHA256.HashData(secret, Encoding.UTF8.GetBytes(stamp + "." + body))));
foreach (var request in new[] {
    Make("delivery-101", "2026-11-03T16:09:00Z"),
    Make("delivery-101", "2026-11-03T16:09:00Z"),
    Make("delivery-102", "2026-10-30T16:00:00Z"),
    Make("delivery-103", "2026-11-03T16:09:10Z")
})
    Console.WriteLine(receiver.Accept(request));
Console.WriteLine($"Applied={receiver.Applied}");

record WebhookRequest(string DeliveryId, string Timestamp, string Body, string Signature);

sealed class WebhookReceiver(byte[] secret, Func<DateTimeOffset> utcNow)
{
    private readonly HashSet<string> processed = new(StringComparer.Ordinal);
    public int Applied { get; private set; }
    public string Accept(WebhookRequest request)
    {
        if (!DateTimeOffset.TryParseExact(request.Timestamp, "yyyy-MM-dd'T'HH:mm:ss'Z'",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var timestamp))
            return "REJECT timestamp";
        var delta = utcNow() - timestamp;
        if (delta < TimeSpan.FromMinutes(-1) || delta > TimeSpan.FromMinutes(5))
            return "REJECT stale";
        if (request.Signature.Length != 64) return "REJECT signature";
        Span<byte> supplied = stackalloc byte[32];
        if (!Convert.TryFromHexString(request.Signature, supplied, out int written) || written != 32)
            return "REJECT signature";
        var expected = HMACSHA256.HashData(secret, Encoding.UTF8.GetBytes(request.Timestamp + "." + request.Body));
        if (!CryptographicOperations.FixedTimeEquals(expected, supplied))
            return "REJECT signature";
        if (!processed.Add(request.DeliveryId)) return "DUPLICATE";
        Applied++;
        return "ACCEPT";
    }
}
