using System.Buffers.Binary;

namespace GateFrameLab;

public readonly record struct GateSample(ushort GateId, ushort EntryCount);

public static class GateFrameDecoder
{
    public static GateSample Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length != 8)
            throw new ArgumentException("A gate frame must be exactly 8 bytes.", nameof(frame));
        if (frame[0] != 0xA5 || frame[1] != 0x01)
            throw new FormatException("Unknown gate frame signature or version.");
        if (frame[6] != 0 || frame[7] != 0)
            throw new FormatException("Reserved bytes must be zero.");

        ushort gateId = BinaryPrimitives.ReadUInt16BigEndian(frame.Slice(2, 2));
        ushort count = BinaryPrimitives.ReadUInt16BigEndian(frame.Slice(4, 2));
        return new GateSample(gateId, count);
    }
}
