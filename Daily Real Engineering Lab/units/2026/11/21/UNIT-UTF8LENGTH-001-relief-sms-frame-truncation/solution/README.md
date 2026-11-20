# Reference solution — UNIT-UTF8LENGTH-001

## Root cause
`message.Length` đếm UTF-16 code units; protocol định nghĩa độ dài theo **byte UTF-8**. Header ngắn hơn payload làm reader dừng giữa một chuỗi multibyte và đọc nhầm các byte còn lại thành header tiếp theo.

## Sửa
Trong writer, encode trước rồi ghi `payload.Length` vào 4-byte header. Bản tham khảo ở `Program.cs`.

## Kiểm chứng
Chạy `./verify.ps1` sau khi chỉnh sửa starter. Thử thêm emoji (surrogate pair), bản tin rỗng và chuỗi ASCII; kiểm tra `ReadExactly` khi frame bị cắt và giới hạn kích thước frame.

## Wrong fixes
- Đổi `Encoding.UTF8` sang ASCII: mất ký tự tiếng Việt.
- Trim hoặc bỏ dấu: làm thay đổi nội dung nghiệp vụ.
- Reader tự đọc thêm đến khi hợp lệ: phá ranh giới giữa các frame và dễ gây desynchronization.
- Chỉ tăng buffer: không sửa metadata của frame.

## Trade-offs
Length-prefix framing cần giới hạn kích thước trước khi cấp phát; UTF-8 strict decoding giúp phát hiện byte lỗi. Trong production, thêm version, checksum hoặc authentication theo threat model và test fragmented reads.
