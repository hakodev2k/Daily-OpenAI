# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms & Evidence
Với `FragmentedStream(maxChunk=2)`, lần đọc header đầu chỉ nhận 2/4 byte; header bị giải mã thiếu. Khi header đủ mà payload đọc ngắn, buffer còn byte 0 mặc dù sender chưa kết thúc.

## Root cause
`Stream.ReadAsync` có thể hoàn thành với ít byte hơn số lượng được yêu cầu mà chưa phải EOF. Starter không sử dụng số byte trả về.

## Why the fix works
Thay cả hai lần đọc bằng `ReadExactlyAsync`, API chỉ hoàn thành khi buffer đã được điền đủ; nếu EOF trước khi đủ sẽ ném `EndOfStreamException`. Giới hạn frame 1024 byte vẫn được giữ.

## How to verify
Sửa `starter/Program.cs` tương đương `solution/Program.cs`; chạy `./verify.ps1`. Script kiểm tra chính starter đã chỉnh sửa, không chạy solution. Dùng `./reproduce.ps1` chỉ trước khi sửa.

## Alternative fixes
Viết vòng lặp dùng số byte trả về và offset; xử lý `0` như EOF. Với TCP thực tế, đọc header theo endian đã thỏa thuận (fixture này little-endian).

## Wrong / tempting fixes
- Tăng buffer size không đảm bảo một lần đọc trả đủ.
- `Task.Delay` không thay đổi contract stream.
- Bỏ kiểm tra frame thiếu có thể tạo dữ liệu giả hợp lệ.
- Đọc tới EOF để lấy payload sẽ chờ sai ranh giới khi kết nối giữ mở.

## Production implications & trade-offs
Đặt giới hạn kích thước trước khi cấp phát; áp dụng cancellation và deadline ở tầng kết nối; log frame ID và byte count thay vì payload nhạy cảm. `ReadExactlyAsync` ngắn gọn, còn vòng lặp thủ công hữu ích khi cần thống kê chi tiết.

## Senior engineer should notice
TCP là byte stream, không phải message transport. Phải thiết kế framing, EOF, endian, giới hạn độ dài và kiểm thử phân mảnh như contract độc lập với máy gửi.
