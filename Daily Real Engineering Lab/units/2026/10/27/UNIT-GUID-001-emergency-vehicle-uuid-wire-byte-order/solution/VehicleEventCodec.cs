namespace GuidWireLab;
// Reference only: copy over starter/VehicleEventCodec.cs after your own attempt.
public static class VehicleEventCodec
{
    public static byte[] EncodeVehicleId(Guid vehicleId) => vehicleId.ToByteArray(bigEndian: true);
    public static Guid DecodeVehicleId(ReadOnlySpan<byte> payload)
    {
        if (payload.Length != 16)
            throw new ArgumentException("Vehicle UUID must occupy exactly 16 bytes.", nameof(payload));
        return new Guid(payload, bigEndian: true);
    }
}
