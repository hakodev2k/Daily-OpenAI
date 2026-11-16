# Reference Solution — UNIT-STREAMREAD-001

## Root cause
`Stream.Read` có quyền trả về ít byte hơn yêu cầu dù chưa đến EOF. Starter đọc một lần, sau đó giải mã toàn bộ buffer 4 byte; các byte chưa được điền giữ giá trị 0. Fixture cố định chunk size 2 nên lỗi luôn tái hiện.

## Fix
Dùng `source.ReadExactly(header)` trong .NET 8 rồi giải mã; xem `solution/Program.cs`. Với runtime cũ, loop đọc tiếp từ offset đã nhận, kiểm tra 0-byte EOF và ném `EndOfStreamException` khi header chưa đủ.

## Verify
Sửa **starter**, không thay fixture. Chạy `./verify.ps1`, kỳ vọng `bytes-read=4`, `decoded-sequence=305419896` và exit code 0. Sau fix `./reproduce.ps1` phải không còn xác nhận symptom.

## Wrong fixes
- Tăng buffer không bảo đảm một lần Read nhận đủ.
- Sleep rồi đọc lại từ offset 0 có thể ghi đè dữ liệu cũ.
- Bù các byte chưa nhận bằng 0 che lỗi transport.
- Verify chỉ chạy solution không kiểm tra thay đổi của learner.

## Trade-offs
`ReadExactly` báo EOF sớm thay vì trả frame giả. Trong production cần thêm length cap, checksum, timeout/cancellation khi đọc async, và phân biệt EOF hợp lệ với truncated frame.

## Senior extension
Thêm magic/version/payload length/checksum; xử lý frame bị cắt, oversized length, nhiều frame trên cùng connection.
