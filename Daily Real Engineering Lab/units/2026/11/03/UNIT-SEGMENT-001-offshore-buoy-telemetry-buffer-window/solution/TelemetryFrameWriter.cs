using System.Buffers.Binary;

namespace TelemetryLab;

internal static class TelemetryFrameWriter
{
    public static byte[] Encode(ArraySegment<byte> payload)
    {
        if (payload.Array is null)
            throw new ArgumentException("A backing buffer is required.", nameof(payload));
        if (payload.Count > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(payload), "Frame exceeds protocol limit.");

        byte[] frame = new byte[sizeof(ushort) + payload.Count];
        BinaryPrimitives.WriteUInt16BigEndian(frame.AsSpan(0, 2), (ushort)payload.Count);
        Buffer.BlockCopy(payload.Array, payload.Offset, frame, 2, payload.Count);
        return frame;
    }
}
