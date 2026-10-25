# Hint 3 — Khái niệm gần với giải pháp

Cần cơ chế tiếp tục đọc cho đến khi đủ số byte yêu cầu, phân biệt EOF sớm với cancellation. Xem `Stream.ReadExactlyAsync` trên .NET 8 hoặc một vòng lặp read có offset và kiểm tra 0-byte EOF. Giữ nguyên giới hạn độ dài frame.
