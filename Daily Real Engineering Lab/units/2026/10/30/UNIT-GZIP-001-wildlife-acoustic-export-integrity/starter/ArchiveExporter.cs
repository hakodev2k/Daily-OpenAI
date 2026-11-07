using System.IO.Compression;

namespace WildlifeArchive;

public static class ArchiveExporter
{
    // Artifact được upload ngay sau khi phương thức này trả về.
    public static byte[] Compress(ReadOnlySpan<byte> payload)
    {
        using var output = new MemoryStream();
        using var compressor = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true);

        compressor.Write(payload);
        compressor.Flush();
        return output.ToArray();
    }
}
