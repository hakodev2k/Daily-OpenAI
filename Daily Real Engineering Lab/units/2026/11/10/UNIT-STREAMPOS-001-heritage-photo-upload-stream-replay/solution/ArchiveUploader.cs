using System.Security.Cryptography;
public sealed record UploadReceipt(string Sha256, byte[] StoredBytes, long SourcePosition);
public sealed class ArchiveUploader
{
    public async Task<UploadReceipt> UploadAsync(Stream source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!source.CanRead) throw new ArgumentException("Source must be readable", nameof(source));
        if (source.CanSeek)
        {
            long start = source.Position;
            string hash = Convert.ToHexString(await SHA256.HashDataAsync(source, cancellationToken));
            source.Position = start;
            using var destination = new MemoryStream();
            await source.CopyToAsync(destination, cancellationToken);
            return new UploadReceipt(hash, destination.ToArray(), source.Position);
        }
        const int limit = 8 * 1024 * 1024;
        using var buffer = new MemoryStream();
        byte[] chunk = new byte[81920];
        int n;
        while ((n = await source.ReadAsync(chunk, cancellationToken)) > 0)
        {
            if (buffer.Length + n > limit) throw new InvalidOperationException("Non-seekable upload exceeds 8 MiB.");
            await buffer.WriteAsync(chunk.AsMemory(0,n), cancellationToken);
        }
        var bytes = buffer.ToArray();
        return new UploadReceipt(Convert.ToHexString(SHA256.HashData(bytes)), bytes, -1);
    }
}
