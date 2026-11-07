using System.IO.Compression;

namespace WildlifeArchive;

public static class ArchiveExporter
{
    public static byte[] Compress(ReadOnlySpan<byte> payload)
    {
        using var output = new MemoryStream();
        using (var compressor = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            compressor.Write(payload);
        }

        return output.ToArray();
    }
}
