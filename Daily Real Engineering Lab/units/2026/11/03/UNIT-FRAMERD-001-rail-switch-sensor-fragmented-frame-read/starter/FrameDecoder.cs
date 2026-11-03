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
        int headerRead = await stream.ReadAsync(header.AsMemory(), cancellationToken);
        if (headerRead != header.Length)
            throw new EndOfStreamException($"Frame header incomplete: {headerRead}/{header.Length} bytes.");

        int payloadLength = BinaryPrimitives.ReadInt32BigEndian(header);
        if (payloadLength < 0 || payloadLength > MaxPayloadBytes)
            throw new InvalidDataException($"Invalid payload length: {payloadLength}.");

        var payload = new byte[payloadLength];
        int payloadRead = await stream.ReadAsync(payload.AsMemory(), cancellationToken);
        if (payloadRead != payloadLength)
            throw new EndOfStreamException($"Frame payload incomplete: {payloadRead}/{payloadLength} bytes.");

        return Encoding.UTF8.GetString(payload);
    }
}
