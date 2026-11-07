using System.Security.Cryptography;

public sealed class WebhookVerifier
{
    private const int MaxBodyBytes = 64 * 1024;
    private readonly byte[] _key;
    public WebhookVerifier(byte[] key) => _key = key.ToArray();

    public bool Verify(byte[] receivedBody, string signatureHex)
    {
        if (receivedBody.Length is 0 or > MaxBodyBytes || signatureHex.Length != 64)
            return false;
        byte[] expected;
        try { expected = Convert.FromHexString(signatureHex); }
        catch (FormatException) { return false; }
        byte[] computed = HMACSHA256.HashData(_key, receivedBody);
        return CryptographicOperations.FixedTimeEquals(computed, expected);
    }
}
