using System.Text;

public sealed class FrameReceiver
{
    public bool TryAccept(UploadEnvelope upload, SensorPacket expected, out string reason)
    {
        if (upload.DeclaredLength != upload.Payload.Length)
        {
            reason = "transport-length-mismatch";
            return false;
        }

        try
        {
            using var stream = new MemoryStream(upload.Payload, writable: false);
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

            var marker = reader.ReadByte();
            var sensorId = reader.ReadString();
            var temperature = reader.ReadInt32();

            if (marker != 0xA7 || sensorId != expected.SensorId ||
                temperature != expected.TemperatureCelsius)
            {
                reason = "payload-field-mismatch";
                return false;
            }

            if (stream.Position != stream.Length)
            {
                reason = "trailing-frame-bytes";
                return false;
            }

            reason = "accepted";
            return true;
        }
        catch (IOException)
        {
            reason = "malformed-frame";
            return false;
        }
        catch (ArgumentException)
        {
            reason = "invalid-frame";
            return false;
        }
    }
}
