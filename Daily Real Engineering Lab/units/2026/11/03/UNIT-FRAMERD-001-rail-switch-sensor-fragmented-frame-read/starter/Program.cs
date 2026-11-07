using System.Buffers.Binary;
using System.Text;

namespace FrameLab;

internal static class Program
{
    private const string Sample = "SWITCH-17|POSITION=NORMAL";

    private static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
                return 2;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "demo":
                    Console.WriteLine(await FrameDecoder.ReadAsync(new MemoryStream(Frame(Sample))));
                    return 0;
                case "reproduce":
                    await ReproduceBaselineAsync();
                    return 0;
                case "verify":
                    await VerifyLearnerAsync();
                    Console.WriteLine("PASS: all learner-code contract cases.");
                    return 0;
                default:
                    Console.Error.WriteLine("Unknown mode.");
                    return 2;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static async Task ReproduceBaselineAsync()
    {
        var wire = Frame(Sample);
        string contiguous = await BaselineDecoder.ReadAsync(new MemoryStream(wire));
        AssertEqual(Sample, contiguous, "baseline contiguous input");

        try
        {
            await BaselineDecoder.ReadAsync(new FragmentedStream(wire, maxBytesPerRead: 2));
        }
        catch (EndOfStreamException ex)
        {
            Console.WriteLine($"REPRODUCED: same {wire.Length}-byte wire frame fails on fragmented stream: {ex.Message}");
            return;
        }

        throw new Exception("Baseline unexpectedly accepted fragmented input; symptom not reproduced.");
    }

    private static async Task VerifyLearnerAsync()
    {
        await AssertDecodedAsync(new MemoryStream(Frame(Sample)), Sample, "contiguous frame");
        await AssertDecodedAsync(new FragmentedStream(Frame(Sample), 2), Sample, "fragmented header and payload");
        await AssertDecodedAsync(new FragmentedStream(Frame(""), 1), "", "zero-length payload");
        await AssertDecodedAsync(new FragmentedStream(Frame("OK"), 1), "OK", "single-byte fragments");

        await ExpectAsync<EndOfStreamException>(
            () => FrameDecoder.ReadAsync(new FragmentedStream(new byte[] { 0, 0, 0 }, 2)),
            "truncated header");

        byte[] truncated = Frame(Sample)[..^2];
        await ExpectAsync<EndOfStreamException>(
            () => FrameDecoder.ReadAsync(new FragmentedStream(truncated, 3)),
            "truncated payload");

        await ExpectAsync<InvalidDataException>(
            () => FrameDecoder.ReadAsync(new MemoryStream(Header(FrameDecoder.MaxPayloadBytes + 1))),
            "oversized frame");

        await ExpectAsync<InvalidDataException>(
            () => FrameDecoder.ReadAsync(new MemoryStream(Header(-1))),
            "negative length");

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        await ExpectAsync<OperationCanceledException>(
            () => FrameDecoder.ReadAsync(new FragmentedStream(Frame(Sample), 2), cancelled.Token),
            "pre-cancelled read");
    }

    private static async Task AssertDecodedAsync(Stream stream, string expected, string caseName)
    {
        using (stream)
        {
            string actual = await FrameDecoder.ReadAsync(stream);
            AssertEqual(expected, actual, caseName);
            Console.WriteLine($"PASS: {caseName}");
        }
    }

    private static async Task ExpectAsync<TException>(Func<Task> action, string caseName)
        where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException)
        {
            Console.WriteLine($"PASS: {caseName}");
            return;
        }

        throw new Exception($"{caseName}: expected {typeof(TException).Name}.");
    }

    private static void AssertEqual(string expected, string actual, string caseName)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
            throw new Exception($"{caseName}: expected '{expected}', got '{actual}'.");
    }

    private static byte[] Header(int payloadLength)
    {
        var header = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(header, payloadLength);
        return header;
    }

    private static byte[] Frame(string text)
    {
        byte[] payload = Encoding.UTF8.GetBytes(text);
        byte[] frame = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32BigEndian(frame.AsSpan(0, 4), payload.Length);
        payload.CopyTo(frame.AsSpan(4));
        return frame;
    }

    // Frozen baseline: reproduce never depends on edits to FrameDecoder.cs.
    private static class BaselineDecoder
    {
        public static async Task<string> ReadAsync(Stream stream, CancellationToken token = default)
        {
            using (stream)
            {
                var header = new byte[4];
                int n = await stream.ReadAsync(header.AsMemory(), token);
                if (n != 4) throw new EndOfStreamException($"Header received {n}/4 bytes.");

                int length = BinaryPrimitives.ReadInt32BigEndian(header);
                if (length < 0 || length > FrameDecoder.MaxPayloadBytes)
                    throw new InvalidDataException("Invalid frame length.");

                var payload = new byte[length];
                int received = await stream.ReadAsync(payload.AsMemory(), token);
                if (received != length)
                    throw new EndOfStreamException($"Payload received {received}/{length} bytes.");

                return Encoding.UTF8.GetString(payload);
            }
        }
    }

    // Deterministic stream: every read returns at most N bytes even if more are available.
    private sealed class FragmentedStream : Stream
    {
        private readonly MemoryStream _inner;
        private readonly int _maxBytesPerRead;

        public FragmentedStream(byte[] bytes, int maxBytesPerRead)
        {
            if (maxBytesPerRead < 1) throw new ArgumentOutOfRangeException(nameof(maxBytesPerRead));
            _inner = new MemoryStream(bytes, writable: false);
            _maxBytesPerRead = maxBytesPerRead;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) =>
            _inner.Read(buffer, offset, Math.Min(count, _maxBytesPerRead));
        public override int Read(Span<byte> buffer) =>
            _inner.Read(buffer[..Math.Min(buffer.Length, _maxBytesPerRead)]);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(Read(buffer.Span));
        }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        protected override void Dispose(bool disposing)
        {
            if (disposing) _inner.Dispose();
            base.Dispose(disposing);
        }
    }
}
