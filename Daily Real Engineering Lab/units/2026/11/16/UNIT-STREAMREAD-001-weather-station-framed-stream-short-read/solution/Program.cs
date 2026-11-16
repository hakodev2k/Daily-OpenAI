using System.Buffers.Binary;
const int Expected = 0x12345678;
byte[] frame = new byte[4];
BinaryPrimitives.WriteInt32LittleEndian(frame, Expected);
using var source = new FragmentedStream(frame, 2);
byte[] header = new byte[4];
source.ReadExactly(header); int read = header.Length;
int actual = BinaryPrimitives.ReadInt32LittleEndian(header);
Console.WriteLine($"bytes-read={read}; decoded-sequence={actual}; expected-sequence={Expected}");
bool mismatch = actual != Expected;
if (args.Contains("--reproduce")) return mismatch ? 0 : 1;
if (args.Contains("--verify")) return !mismatch && read == 4 ? 0 : 1;
return 0;
sealed class FragmentedStream : MemoryStream {
    private readonly int max;
    public FragmentedStream(byte[] data, int chunk) : base(data, false) => max = chunk;
    public override int Read(byte[] b, int offset, int count) => base.Read(b, offset, Math.Min(count,max));
    public override int Read(Span<byte> b) => base.Read(b[..Math.Min(b.Length,max)]);
}
