using System.Security.Cryptography;

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

        var expected = HMACSHA256.HashData(secret, body);
        return supplied.Length == expected.Length &&
               CryptographicOperations.FixedTimeEquals(supplied, expected);
    }
}
