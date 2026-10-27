using System.Security.Cryptography;
using System.Text;
using EnvelopeIncident;

return args.FirstOrDefault() switch
{
    "--reproduce" => Reproduce(),
    "--verify" => Verify(),
    "--demo" or null => Demo(),
    _ => 4
};

static byte[] Bytes(string value) => Encoding.UTF8.GetBytes(value);

static int Demo()
{
    using var key = KeyMaterial.Create();
    var first = EnvelopeCrypto.Encrypt(key.Bytes, "coldstore-2026-10", Bytes("SENSOR-17|temperature=2.0|ok"));
    var second = EnvelopeCrypto.Encrypt(key.Bytes, "coldstore-2026-10", Bytes("SENSOR-18|temperature=3.0|ok"));
    Console.WriteLine("BATCH=coldstore-2026-10 RECORDS=2");
    Console.WriteLine($"ENVELOPE_1_NONCE={Convert.ToHexString(first.Nonce)}");
    Console.WriteLine($"ENVELOPE_2_NONCE={Convert.ToHexString(second.Nonce)}");
    Console.WriteLine($"DECRYPTED_1={Encoding.UTF8.GetString(EnvelopeCrypto.Decrypt(key.Bytes, "coldstore-2026-10", first))}");
    Console.WriteLine($"DECRYPTED_2={Encoding.UTF8.GetString(EnvelopeCrypto.Decrypt(key.Bytes, "coldstore-2026-10", second))}");
    return 0;
}

static int Reproduce()
{
    try
    {
        using var key = KeyMaterial.Create();
        const string batch = "coldstore-2026-10";
        byte[] p1 = Bytes("SENSOR-17|temperature=2.0|ok");
        byte[] p2 = Bytes("SENSOR-18|temperature=3.0|ok");
        var a = EnvelopeCrypto.Encrypt(key.Bytes, batch, p1);
        var b = EnvelopeCrypto.Encrypt(key.Bytes, batch, p2);
        bool decrypts = EnvelopeCrypto.Decrypt(key.Bytes, batch, a).SequenceEqual(p1)
            && EnvelopeCrypto.Decrypt(key.Bytes, batch, b).SequenceEqual(p2);
        bool sameField = a.Nonce.SequenceEqual(b.Nonce);
        bool relationExposed = a.Ciphertext.Length == b.Ciphertext.Length
            && Enumerable.Range(0, p1.Length).All(i =>
                (a.Ciphertext[i] ^ b.Ciphertext[i]) == (p1[i] ^ p2[i]));
        Console.WriteLine($"ROUNDTRIP_OK={decrypts}");
        Console.WriteLine($"REPEATED_ENVELOPE_FIELD={sameField}");
        Console.WriteLine($"BYTE_RELATION_EXPOSED={relationExposed}");
        if (decrypts && sameField && relationExposed)
        {
            Console.WriteLine("REPRODUCED: authenticated exports reveal a plaintext relationship.");
            return 0;
        }
        Console.WriteLine("NOT_REPRODUCED: original symptom no longer present.");
        return 2;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"HARNESS_ERROR: {ex.GetType().Name}: {ex.Message}");
        return 3;
    }
}

static int Verify()
{
    try
    {
        using var key = KeyMaterial.Create();
        const string batch = "coldstore-2026-10";
        var uniqueFields = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < 48; i++)
        {
            byte[] input = Bytes($"SENSOR-{i:D2}|temperature={i % 7}.0|ok");
            var env = EnvelopeCrypto.Encrypt(key.Bytes, batch, input);
            if (env.Nonce.Length != 12 || env.Tag.Length != 16 ||
                env.Ciphertext.Length != input.Length ||
                !uniqueFields.Add(Convert.ToHexString(env.Nonce)))
                return Fail("Envelope shape or per-record uniqueness contract failed.");
            if (!EnvelopeCrypto.Decrypt(key.Bytes, batch, env).SequenceEqual(input))
                return Fail("Round-trip failed.");

            var changedTag = env with { Tag = (byte[])env.Tag.Clone() };
            changedTag.Tag[0] ^= 1;
            if (!Rejects(() => EnvelopeCrypto.Decrypt(key.Bytes, batch, changedTag)))
                return Fail("Tampered tag was accepted.");

            var changedCipher = env with { Ciphertext = (byte[])env.Ciphertext.Clone() };
            changedCipher.Ciphertext[0] ^= 1;
            if (!Rejects(() => EnvelopeCrypto.Decrypt(key.Bytes, batch, changedCipher)))
                return Fail("Tampered ciphertext was accepted.");

            if (!Rejects(() => EnvelopeCrypto.Decrypt(key.Bytes, "wrong-batch", env)))
                return Fail("Wrong batch context was accepted.");

            byte[] otherKey = (byte[])key.Bytes.Clone();
            otherKey[0] ^= 1;
            if (!Rejects(() => EnvelopeCrypto.Decrypt(otherKey, batch, env)))
                return Fail("Wrong key was accepted.");
        }
        Console.WriteLine($"UNIQUE_ENVELOPES={uniqueFields.Count}");
        Console.WriteLine("VERIFIED: uniqueness, round-trip and authentication contracts passed.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"HARNESS_ERROR: {ex.GetType().Name}: {ex.Message}");
        return 3;
    }
}

static bool Rejects(Action action)
{
    try { action(); return false; }
    catch (CryptographicException) { return true; }
}

static int Fail(string message)
{
    Console.Error.WriteLine($"CONTRACT_FAIL: {message}");
    return 2;
}

sealed class KeyMaterial : IDisposable
{
    public byte[] Bytes { get; } = RandomNumberGenerator.GetBytes(32);
    public static KeyMaterial Create() => new();
    public void Dispose() => CryptographicOperations.ZeroMemory(Bytes);
}
