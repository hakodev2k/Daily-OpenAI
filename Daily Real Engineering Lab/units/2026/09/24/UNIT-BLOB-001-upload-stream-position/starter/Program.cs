using System.Security.Cryptography;
using System.Text;

var input = Encoding.UTF8.GetBytes("invoice-2026-09-24|customer=ACME|total=1250.00");
using var stream = new MemoryStream(input);

var checksum = await InspectAsync(stream);
var stored = await LocalBlobStore.UploadAsync(stream);

Console.WriteLine($"InputBytes={input.Length}");
Console.WriteLine($"Checksum={checksum}");
Console.WriteLine($"UploadedBytes={stored.Length}");
Console.WriteLine($"PayloadMatches={input.SequenceEqual(stored)}");

static async Task<string> InspectAsync(Stream source)
{
    using var sha = SHA256.Create();
    var hash = await sha.ComputeHashAsync(source);
    return Convert.ToHexString(hash);
}

static class LocalBlobStore
{
    public static async Task<byte[]> UploadAsync(Stream source)
    {
        using var destination = new MemoryStream();
        await source.CopyToAsync(destination);
        return destination.ToArray();
    }
}