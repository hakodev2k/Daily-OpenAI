using System.Buffers.Binary;
using System.Text;

return await Lab.RunAsync(args);

static class Lab
{
    private const string MeterReading = "meter-17|2026-10-25T08:00:00Z|129.7";

    public static async Task<int> RunAsync(string[] args)
    {
        string mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
        try
        {
            return mode switch
            {
                "demo" => await DemoAsync(),
                "reproduce" => await ReproduceAsync(),
                "verify" => await VerifyAsync(),
                _ => Usage()
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{mode} failed: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
        return 2;
    }

    private static async Task<int> DemoAsync()
    {
        foreach (int maxChunk in new[] { 4096, 2 })
        {
            try
            {
                string value = await FrameReader.ReadFrameAsync(
                    new FragmentingStream(BuildFrame(MeterReading), maxChunk),
                    CancellationToken.None);
                Console.WriteLine($"chunk={maxChunk}: accepted {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"chunk={maxChunk}: rejected ({ex.GetType().Name}: {ex.Message})");
            }
        }
        return 0;
    }

    private static async Task<int> ReproduceAsync()
    {
        try
        {
            string value = await FrameReader.ReadFrameAsync(
                new FragmentingStream(BuildFrame(MeterReading), 2),
                CancellationToken.None);
            Console.Error.WriteLine($"NOT REPRODUCED: frame accepted: {value}");
            return 1;
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine($"REPRODUCED: valid frame rejected: {ex.Message}");
            return 0;
        }
    }

    private static async Task<int> VerifyAsync()
    {
        foreach (int maxChunk in new[] { 4096, 1, 2, 3 })
        {
            string actual = await FrameReader.ReadFrameAsync(
                new FragmentingStream(BuildFrame(MeterReading), maxChunk),
                CancellationToken.None);
            if (actual != MeterReading)
                throw new Exception($"Payload mismatch for chunk={maxChunk}: {actual}");
            Console.WriteLine($"PASS valid frame chunk={maxChunk}");
        }

        byte[] frame = BuildFrame(MeterReading);
        byte[] truncated = frame[..^1];
        try
        {
            await FrameReader.ReadFrameAsync(
                new FragmentingStream(truncated, 4096), CancellationToken.None);
            throw new Exception("Truncated frame was accepted");
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("PASS truncated frame rejected at EOF");
        }

        byte[] oversized = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(oversized, 2048);
        try
        {
            await FrameReader.ReadFrameAsync(
                new FragmentingStream(oversized, 4096), CancellationToken.None);
            throw new Exception("Oversized frame was accepted");
        }
        catch (InvalidDataException)
        {
            Console.WriteLine("PASS oversized frame rejected");
        }

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        try
        {
            await FrameReader.ReadFrameAsync(
                new FragmentingStream(frame, 2), cts.Token);
            throw new Exception("Cancelled read was accepted");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("PASS cancellation propagated");
        }

        Console.WriteLine("VERIFY PASSED");
        return 0;
    }

    private static byte[] BuildFrame(string text)
    {
        byte[] payload = Encoding.UTF8.GetBytes(text);
        byte[] frame = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32BigEndian(frame.AsSpan(0, 4), payload.Length);
        payload.AsSpan().CopyTo(frame.AsSpan(4));
        return frame;
    }
}

static class FrameReader
{
    public static async Task<string> ReadFrameAsync(Stream stream, CancellationToken cancellationToken)
    {
        byte[] header = new byte[4];
        int headerRead = await stream.ReadAsync(header.AsMemory(), cancellationToken);
        if (headerRead != header.Length)
            throw new InvalidDataException($"Header incomplete: {headerRead}/{header.Length}");

        int payloadLength = BinaryPrimitives.ReadInt32BigEndian(header);
        if (payloadLength < 1 || payloadLength > 1024)
            throw new InvalidDataException($"Invalid frame length: {payloadLength}");

        byte[] payload = new byte[payloadLength];
        int payloadRead = await stream.ReadAsync(payload.AsMemory(), cancellationToken);
        if (payloadRead != payload.Length)
            throw new InvalidDataException($"Payload incomplete: {payloadRead}/{payload.Length}");

        return Encoding.UTF8.GetString(payload);
    }
}

sealed class FragmentingStream : Stream
{
    private readonly byte[] _data;
    private readonly int _maxChunk;
    private int _position;

    public FragmentingStream(byte[] data, int maxChunk)
    {
        if (maxChunk <= 0) throw new ArgumentOutOfRangeException(nameof(maxChunk));
        _data = data;
        _maxChunk = maxChunk;
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        int remaining = _data.Length - _position;
        int count = Math.Min(buffer.Length, Math.Min(_maxChunk, remaining));
        if (count == 0) return ValueTask.FromResult(0);
        _data.AsMemory(_position, count).CopyTo(buffer);
        _position += count;
        return ValueTask.FromResult(count);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

    public override int Read(byte[] buffer, int offset, int count) =>
        ReadAsync(buffer.AsMemory(offset, count)).GetAwaiter().GetResult();

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => _position; set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
