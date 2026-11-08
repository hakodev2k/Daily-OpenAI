using System.Text;

var mode = args.Length == 0 ? "fragmented" : args[0];
var frame = Encoding.ASCII.GetBytes("WAVE=1234");
var wire = new byte[4 + frame.Length];
BitConverter.TryWriteBytes(wire.AsSpan(0, 4), frame.Length); // local little-endian fixture
frame.CopyTo(wire.AsSpan(4));
if (mode == "truncated") wire = wire[..^2];
if (mode == "empty") wire = [];
using var input = new FragmentedStream(wire, mode == "single" ? 4096 : 2);
try
{
    var payload = await ReadFrameAsync(input);
    Console.WriteLine($"PAYLOAD={Encoding.ASCII.GetString(payload)};BYTES={payload.Length};EXPECTED={frame.Length}");
    if (mode is "truncated" or "empty") { Console.Error.WriteLine("FAIL: incomplete frame accepted"); return 1; }
    if (!payload.SequenceEqual(frame)) { Console.Error.WriteLine("FAIL: frame corrupted"); return 1; }
    Console.WriteLine("PASS"); return 0;
}
catch (EndOfStreamException)
{
    if (mode is "truncated" or "empty") { Console.WriteLine("PASS: incomplete frame rejected"); return 0; }
    Console.Error.WriteLine("FAIL: valid frame rejected"); return 1;
}

static async Task<byte[]> ReadFrameAsync(Stream stream)
{
    var header = new byte[4];
    await stream.ReadAsync(header);
    var length = BitConverter.ToInt32(header);
    if (length is < 1 or > 1024) throw new EndOfStreamException("Invalid frame header");
    var payload = new byte[length];
    await stream.ReadAsync(payload);
    return payload;
}

sealed class FragmentedStream : MemoryStream
{
    private readonly int maxChunk;
    public FragmentedStream(byte[] bytes, int maxChunk) : base(bytes, writable: false) => this.maxChunk = maxChunk;
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var count = Math.Min(buffer.Length, maxChunk);
        var n = base.Read(buffer.Span[..count]);
        Console.WriteLine($"READ requested={buffer.Length} returned={n}");
        return ValueTask.FromResult(n);
    }
}
