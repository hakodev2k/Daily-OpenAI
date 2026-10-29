using System.Buffers;

namespace PreviewLeaseLab;

// Fixture cố định: mô phỏng một hành vi tái sử dụng hợp lệ của ArrayPool.
// Không dùng pool này cho đa luồng hoặc production.
public sealed class RecyclingArrayPool : ArrayPool<byte>
{
    private byte[]? _cached;

    public override byte[] Rent(int minimumLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minimumLength);
        if (_cached is { } buffer && buffer.Length >= minimumLength)
        {
            _cached = null;
            return buffer;
        }

        return new byte[Math.Max(minimumLength, 64)];
    }

    public override void Return(byte[] array, bool clearArray = false)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (clearArray)
            Array.Clear(array);
        _cached = array;
    }
}
