using System.Buffers.Binary;
using System.Text;

namespace FrameLab;

public static class FrameDecoder
{
    public const int MaxPayloadBytes = 4096;

    public static async Task<string> ReadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var header = new byte[sizeof(int)];
        await stream.ReadExactlyAsync(header.AsMemory(), cancellationToken);

        int payloadLength = BinaryPrimitives.ReadInt32BigEndian(header);
        if (payloadLength < 0 || payloadLength > MaxPayloadBytes)
            throw new InvalidDataException($"Invalid payload length: {payloadLength}.");

        var payload = new byte[payloadLength];
        await stream.ReadExactlyAsync(payload.AsMemory(), cancellationToken);

        return Encoding.UTF8.GetString(payload);
    }
}
