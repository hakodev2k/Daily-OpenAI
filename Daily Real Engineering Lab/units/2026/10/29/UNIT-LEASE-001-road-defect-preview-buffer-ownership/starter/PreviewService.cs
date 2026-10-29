using System.Buffers;

namespace PreviewLeaseLab;

// Service chuẩn bị dữ liệu ảnh xem trước để gửi sang hàng đợi xuất bản.
// Caller có thể giữ kết quả trong lúc một yêu cầu khác đang xử lý.
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
            return buffer.AsMemory(0, renderedTile.Length);
        }
        finally
        {
            _pool.Return(buffer);
        }
    }
}
