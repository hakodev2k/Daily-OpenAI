using System.Text;

public sealed record SensorPacket(string SensorId, int TemperatureCelsius, int ReservedCapacity);

public sealed record UploadEnvelope(byte[] Payload, int DeclaredLength);

public sealed class FrameExporter
{
    public UploadEnvelope Build(SensorPacket packet)
    {
        using var buffer = new MemoryStream(packet.ReservedCapacity);
        using (var writer = new BinaryWriter(buffer, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write((byte)0xA7);
            writer.Write(packet.SensorId);
            writer.Write(packet.TemperatureCelsius);
        }

        var payload = buffer.ToArray();
        return new UploadEnvelope(payload, payload.Length);
    }
}
