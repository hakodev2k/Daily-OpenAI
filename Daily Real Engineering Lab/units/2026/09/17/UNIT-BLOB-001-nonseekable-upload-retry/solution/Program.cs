using System.Text;

var expected = Encoding.UTF8.GetBytes("invoice-pdf-content-12345");
await using var source = new NonSeekableReadStream(expected);
var storage = new FakeBlobClient();
var service = new AttachmentService(storage);
await service.UploadWithRetryAsync(source);
Console.WriteLine($"attempts={storage.Attempts}");
Console.WriteLine($"reads={string.Join(',', storage.BytesReadPerAttempt)}");
Console.WriteLine($"stored-bytes={storage.Stored.Length}");
Console.WriteLine($"stored={Encoding.UTF8.GetString(storage.Stored)}");

sealed class AttachmentService(FakeBlobClient storage)
{
    public async Task UploadWithRetryAsync(Stream input)
    {
        using var replay = new MemoryStream();
        await input.CopyToAsync(replay);
        var payload = replay.ToArray();
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                await using var attemptStream = new MemoryStream(payload, writable: false);
                await storage.UploadAsync(attemptStream);
                return;
            }
            catch (TransientStorageException) when (attempt < 2)
            {
                Console.WriteLine("transient failure; retrying");
            }
        }
    }
}

sealed class FakeBlobClient
{
    public int Attempts { get; private set; }
    public List<int> BytesReadPerAttempt { get; } = [];
    public byte[] Stored { get; private set; } = [];
    public async Task UploadAsync(Stream input)
    {
        Attempts++;
        using var copy = new MemoryStream();
        await input.CopyToAsync(copy);
        var bytes = copy.ToArray();
        BytesReadPerAttempt.Add(bytes.Length);
        if (Attempts == 1) throw new TransientStorageException();
        Stored = bytes;
    }
}
sealed class TransientStorageException : Exception;
sealed class NonSeekableReadStream : Stream
{
    private readonly MemoryStream inner;
    public NonSeekableReadStream(byte[] bytes) => inner = new(bytes, writable: false);
    public override bool CanRead => true; public override bool CanSeek => false; public override bool CanWrite => false;
    public override long Length => inner.Length; public override long Position { get => inner.Position; set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => inner.ReadAsync(buffer, cancellationToken);
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(); public override void SetLength(long value) => throw new NotSupportedException(); public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    protected override void Dispose(bool disposing) { if (disposing) inner.Dispose(); base.Dispose(disposing); }
    public override async ValueTask DisposeAsync() { await inner.DisposeAsync(); GC.SuppressFinalize(this); }
}