using System.Buffers;

namespace PreviewLeaseLab;

// Reference: chuyển dữ liệu từ vùng nhớ thuê sang vùng nhớ do kết quả sở hữu.
// ReadOnlyMemory chỉ hạn chế API ghi, không tự đảm bảo ownership.
public sealed class PreviewService
{
    private readonly ArrayPool<byte> _pool;

    public PreviewService(ArrayPool<byte> pool) => _pool = pool;

    public ReadOnlyMemory<byte> Prepare(ReadOnlySpan<byte> renderedTile)
    {
        byte[] buffer = _pool.Rent(renderedTile.Length);
        try
        {
            renderedTile.CopyTo(buffer);
            byte[] owned = buffer.AsSpan(0, renderedTile.Length).ToArray();
            return owned;
        }
        finally
        {
            _pool.Return(buffer);
        }
    }
}
