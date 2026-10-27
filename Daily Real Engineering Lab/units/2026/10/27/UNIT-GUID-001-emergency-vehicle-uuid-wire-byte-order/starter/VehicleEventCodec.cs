namespace GuidWireLab;
public static class VehicleEventCodec
{
    // Partner contract: exactly 16 UUID bytes per frame.
    public static byte[] EncodeVehicleId(Guid vehicleId) => vehicleId.ToByteArray();
    public static Guid DecodeVehicleId(ReadOnlySpan<byte> payload)
    {
        if (payload.Length != 16)
            throw new ArgumentException("Vehicle UUID must occupy exactly 16 bytes.", nameof(payload));
        return new Guid(payload);
    }
}
