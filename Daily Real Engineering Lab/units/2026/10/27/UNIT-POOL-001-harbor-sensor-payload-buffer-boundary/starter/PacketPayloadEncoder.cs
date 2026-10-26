using System.Buffers;
namespace TelemetryRelay;
public sealed class PacketPayloadEncoder
{
    private readonly ArrayPool<byte> _pool;
    public PacketPayloadEncoder(ArrayPool<byte> pool) =>
        _pool = pool ?? throw new ArgumentNullException(nameof(pool));

    public string Encode(Stream source, int declaredLength)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (declaredLength is < 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(declaredLength));
        byte[] rented = _pool.Rent(Math.Max(1, declaredLength));
        try
        {
            source.ReadExactly(rented.AsSpan(0, declaredLength));
            return Convert.ToBase64String(rented);
        }
        finally
        {
            _pool.Return(rented, clearArray: true);
        }
    }
}
