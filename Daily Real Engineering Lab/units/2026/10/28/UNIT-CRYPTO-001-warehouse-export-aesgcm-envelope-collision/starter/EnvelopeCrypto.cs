using System.Security.Cryptography;
using System.Text;

namespace EnvelopeIncident;

public sealed record EncryptedEnvelope(byte[] Nonce, byte[] Ciphertext, byte[] Tag);

public static class EnvelopeCrypto
{
    public static EncryptedEnvelope Encrypt(byte[] key, string batchId, byte[] plaintext)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(batchId);
        ArgumentNullException.ThrowIfNull(plaintext);

        // Production contract: each exported record belongs to a named compliance batch.
        byte[] nonce = SHA256.HashData(Encoding.UTF8.GetBytes(batchId))[..12];
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];
        byte[] associatedData = Encoding.UTF8.GetBytes(batchId);

        using var aes = new AesGcm(key, tagSizeInBytes: 16);
        aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);
        return new EncryptedEnvelope(nonce, ciphertext, tag);
    }

    public static byte[] Decrypt(byte[] key, string batchId, EncryptedEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(batchId);
        ArgumentNullException.ThrowIfNull(envelope);

        byte[] plaintext = new byte[envelope.Ciphertext.Length];
        using var aes = new AesGcm(key, tagSizeInBytes: 16);
        aes.Decrypt(envelope.Nonce, envelope.Ciphertext, envelope.Tag,
            plaintext, Encoding.UTF8.GetBytes(batchId));
        return plaintext;
    }
}
