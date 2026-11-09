using System.Buffers.Binary;

public static class RiverFrameDecoder
{
    // Wire contract: [0..1] station ID (big-endian unsigned);
    // [2..3] water temperature in 0.1°C (little-endian signed);
    // [4..7] sample sequence (big-endian unsigned).
    public static Reading Decode(ReadOnlySpan<byte> frame)
    {
        if (frame.Length != 8)
            throw new ArgumentException("Frame must be exactly 8 bytes.", nameof(frame));

        ushort station = BinaryPrimitives.ReadUInt16BigEndian(frame[..2]);
        short deciCelsius = BinaryPrimitives.ReadInt16LittleEndian(frame.Slice(2, 2));
        uint sequence = BinaryPrimitives.ReadUInt32BigEndian(frame.Slice(4, 4));
        return new Reading(station, deciCelsius, sequence);
    }
}

public readonly record struct Reading(ushort StationId, short TemperatureDeciC, uint Sequence);
