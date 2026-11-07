using System.Text;

public sealed record SensorPacket(string SensorId, int TemperatureCelsius, int ReservedCapacity);

public sealed record UploadEnvelope(byte[] Payload, int DeclaredLength);

public sealed class FrameExporter
{
    // Wire contract: 0xA7, BinaryWriter UTF-8 string, little-endian Int32 temperature.
    public UploadEnvelope Build(SensorPacket packet)
    {
        using var buffer = new MemoryStream(packet.ReservedCapacity);
        using (var writer = new BinaryWriter(buffer, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write((byte)0xA7);
            writer.Write(packet.SensorId);
            writer.Write(packet.TemperatureCelsius);
        }

        // This envelope is passed to a transport that sends Payload in full.
        return new UploadEnvelope(buffer.GetBuffer(), checked((int)buffer.Length));
    }
}
