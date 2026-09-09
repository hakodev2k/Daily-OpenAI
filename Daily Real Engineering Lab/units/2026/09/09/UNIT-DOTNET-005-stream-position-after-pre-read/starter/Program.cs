using System.Security.Cryptography;
using System.Text;

var mode = args.FirstOrDefault() ?? "reproduce";
var payload = Encoding.UTF8.GetBytes("contract-2026-09-09|customer=42|version=7");
await using var input = new MemoryStream(payload, writable: false);

Console.WriteLine($"Input Length={input.Length}, Position={input.Position}");

var hash = await IntegrityHasher.ComputeSha256Async(input);
Console.WriteLine($"After integrity check Length={input.Length}, Position={input.Position}");
Console.WriteLine($"SHA256={hash}");

var storage = new InMemoryBlobStorage();
var stored = await storage.UploadAsync(input);

Console.WriteLine($"Storage copied bytes={stored.Length}");
Console.WriteLine($"After upload Length={input.Length}, Position={input.Position}");

var storedHash = Convert.ToHexString(SHA256.HashData(stored));
var contentMatches = payload.SequenceEqual(stored);
var hashMatches = string.Equals(hash, storedHash, StringComparison.Ordinal);

Console.WriteLine($"Content matches={contentMatches}");
Console.WriteLine($"Hash matches={hashMatches}");

if (mode.Equals("reproduce", StringComparison.OrdinalIgnoreCase))
{
    if (stored.Length == payload.Length && contentMatches)
    {
        Console.Error.WriteLine("REPRODUCTION_FAILED: starter no longer exhibits the intended symptom.");
        return 2;
    }

    Console.WriteLine("REPRODUCTION_OK: integrity check succeeded but storage did not receive the full payload.");
    return 0;
}

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    if (stored.Length != payload.Length || !contentMatches || !hashMatches)
    {
        Console.Error.WriteLine("VERIFICATION_FAILED: stored payload or integrity evidence is incorrect.");
        return 1;
    }

    Console.WriteLine("VERIFICATION_OK: storage received the full payload and integrity verification still matches.");
    return 0;
}

Console.Error.WriteLine("Usage: dotnet run --project starter -- reproduce|verify");
return 64;

static class IntegrityHasher
{
    public static async Task<string> ComputeSha256Async(Stream source)
    {
        using var sha = SHA256.Create();
        var bytes = await sha.ComputeHashAsync(source);
        return Convert.ToHexString(bytes);
    }
}

sealed class InMemoryBlobStorage
{
    public async Task<byte[]> UploadAsync(Stream source)
    {
        await using var destination = new MemoryStream();
        await source.CopyToAsync(destination);
        return destination.ToArray();
    }
}
