using System.Security.Cryptography;
using System.Text.Json;

public static class WebhookVerifier
{
    public static bool Verify(byte[] body, string signatureHex, byte[] secret)
    {
        if (body is null || secret is null || string.IsNullOrWhiteSpace(signatureHex))
            return false;

        byte[] supplied;
        try
        {
            supplied = Convert.FromHexString(signatureHex);
        }
        catch (FormatException)
        {
            return false;
        }

        // Investigation: is this intermediate representation identical to
        // the representation at the external integration boundary?
        using var document = JsonDocument.Parse(body);
        var normalized = JsonSerializer.SerializeToUtf8Bytes(document.RootElement);
        var expected = HMACSHA256.HashData(secret, normalized);

        return supplied.Length == expected.Length &&
               CryptographicOperations.FixedTimeEquals(supplied, expected);
    }
}
