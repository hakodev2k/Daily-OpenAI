using System.Security.Cryptography;
public sealed record UploadReceipt(string Sha256, byte[] StoredBytes, long SourcePosition);
public sealed class ArchiveUploader
{
    public async Task<UploadReceipt> UploadAsync(Stream source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        var hash = Convert.ToHexString(await SHA256.HashDataAsync(source, cancellationToken));
        using var destination = new MemoryStream();
        await source.CopyToAsync(destination, cancellationToken);
        return new UploadReceipt(hash, destination.ToArray(), source.CanSeek ? source.Position : -1);
    }
}
