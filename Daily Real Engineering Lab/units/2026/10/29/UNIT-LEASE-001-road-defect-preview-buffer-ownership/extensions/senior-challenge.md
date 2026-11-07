# Senior Challenge — giới hạn ownership ở pipeline async
Thiết kế API dùng IMemoryOwner<byte> để không copy trên đường nóng. Xác định rõ:
- ai Dispose khi publish thành công, timeout, cancellation, exception và retry;
- cách đảm bảo không double-return hoặc use-after-dispose;
- cách giới hạn số byte đang giữ khi queue tăng;
- contract test với consumer đọc trễ và nhiều producer.
Không sửa fixture chính trước khi verify bài gốc. So sánh allocation với độ phức tạp vận hành.
