# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms và Evidence
Với mọi payload trong ba test case, `wireLength=256`, trong khi frame hợp lệ phải có độ dài `payloadLength+2`. Receiver đọc length prefix 2 byte nhưng từ chối trailing padding.

## Root cause
`MemoryStream.GetBuffer()` trả về toàn bộ backing array, kích thước bằng `Capacity`, không phải `Length`. Ở đây constructor đặt capacity 256; dữ liệu chỉ chiếm 2, 7 hoặc 202 byte. Phần chưa ghi được gửi cùng mảng.

## Reference fix
Trong `starter/Program.cs`, đổi dòng `return stream.GetBuffer();` thành `return stream.ToArray();`. `ToArray()` tạo bản sao chỉ gồm số byte đã ghi.

## Verify
Chạy `./verify.ps1` sau khi sửa starter. Cả 0, 5, 200 byte phải có `accepted=True` và `wireLength=payloadLength+2`. Chạy `./reproduce.ps1` trước khi sửa, không dùng nó làm tiêu chí pass sau sửa.

## Alternative fixes và trade-offs
Nếu transport nhận `ReadOnlyMemory<byte>` hoặc `ArraySegment<byte>`, có thể dùng một view chỉ gồm `stream.Length` byte và kiểm soát lifetime. Cách này tránh copy nhưng đòi hỏi ownership rõ ràng; không được dispose hoặc tái sử dụng buffer trước khi send hoàn tất.

## Wrong / tempting fixes
- Tăng capacity chỉ làm padding lớn hơn.
- Trim các byte 00 cuối frame làm mất dữ liệu hợp lệ khi payload thực sự kết thúc bằng 00.
- Sửa receiver bỏ qua byte thừa có thể che giấu lỗi giao thức và tạo khác biệt giữa các client.

## Production implications
Wire contract phải nêu rõ length prefix, byte order, giới hạn kích thước và hành vi với zero-byte payload. Test tại ranh giới serialize/transport, không chỉ test nội dung buffer đã ghi.

## Senior engineer nên nhận ra
`Capacity` là tài nguyên cấp phát, `Length` là dữ liệu logic. API trả về backing storage cần được kiểm tra ownership và valid slice. Thêm property-based tests cho nhiều kích thước và payload có trailing zero.
