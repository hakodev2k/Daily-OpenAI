using System.Security.Cryptography;
using System.Text;

internal static class Program
{
    private static async Task<(string checksum, byte[] stored)> ArchiveAsync(Stream upload)
    {
        using var sha = SHA256.Create();
        var checksum = Convert.ToHexString(await sha.ComputeHashAsync(upload));
        using var archive = new MemoryStream();
        await upload.CopyToAsync(archive);
        return (checksum, archive.ToArray());
    }

    public static async Task<int> Main(string[] args)
    {
        var payload = Encoding.UTF8.GetBytes("WILDLIFE-AUDIO-RECORDING-2026");
        using var upload = new MemoryStream(payload, writable: false);
        var (checksum, stored) = await ArchiveAsync(upload);
        var checksumOk = checksum == Convert.ToHexString(SHA256.HashData(payload));
        var payloadOk = payload.AsSpan().SequenceEqual(stored);
        Console.WriteLine($"CHECKSUM_OK={checksumOk}");
        Console.WriteLine($"STORED_BYTES={stored.Length}");
        Console.WriteLine($"PAYLOAD_OK={payloadOk}");
        var broken = checksumOk && !payloadOk;
        var mode = args.FirstOrDefault() ?? "observe";
        if (mode == "reproduce") return broken ? 0 : 1;
        if (mode == "verify") return checksumOk && payloadOk ? 0 : 1;
        return 0;
    }
}
