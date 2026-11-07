# Reference Solution — UNIT-IO-001

> Chỉ đọc sau khi đã reproduce và thử sửa starter.

## 1. Symptoms
Frame hợp lệ được xử lý khi stream trả nhiều byte trong một lần, nhưng bị reject khi stream trả từng phần. Replay có thể khiến sự cố biến mất.

## 2. Evidence
`demo` so sánh cùng một frame với `maxChunk=4096` và `maxChunk=2`. `reproduce` xác nhận frame đầy đủ bị reject. `verify` kiểm tra thêm các chunk 1, 2, 3 và các negative cases.

## 3. Root cause
`Stream.ReadAsync(Memory<byte>, CancellationToken)` trả về **số byte hiện có**, có thể nhỏ hơn độ dài buffer dù chưa EOF. Starter giả định một lần đọc sẽ nhận đủ 4 byte header hoặc toàn bộ payload. Với fragmented stream, phép so sánh số byte khiến frame hợp lệ bị loại.

## 4. Why the fix works
Sử dụng `Stream.ReadExactlyAsync` của .NET 8 cho **cả header và payload**. API tiếp tục đọc đến khi đủ byte hoặc ném `EndOfStreamException` khi stream kết thúc sớm. Giữ kiểm tra độ dài 1..1024 trước khi cấp phát payload, giữ `CancellationToken` xuyên suốt. Code tham khảo: [Program.cs](Program.cs).

## 5. How to verify
Chỉnh `starter/Program.cs` rồi chạy `./scripts/verify.ps1`. Phải PASS với chunk 4096/1/2/3; truncated frame phải bị reject ở EOF; oversized length bị reject trước khi cấp phát; cancellation phải lan truyền. `reproduce.ps1` sau khi fix không còn báo lỗi gốc là điều bình thường.

## 6. Alternative fixes
Có thể tự viết vòng lặp `while (offset < buffer.Length)` và gọi `ReadAsync(buffer.AsMemory(offset), token)`; khi kết quả 0 trước khi đủ byte thì ném `EndOfStreamException`. Cách này cần test kỹ hơn so với API framework.

## 7. Wrong or misleading fixes
- Tăng kích thước buffer không buộc stream trả đủ dữ liệu.
- Thêm `Task.Delay` hoặc retry cả frame trên **cùng** stream có thể làm mất đồng bộ vị trí dữ liệu.
- Bỏ kiểm tra số byte để giải mã buffer chưa đầy có thể tạo dữ liệu giả.
- Chỉ sửa header mà quên payload vẫn còn lỗi với frame lớn.

## 8. Production implications
NetworkStream, TLS stream, file stream và proxy có thể trả dữ liệu theo từng phần; không được coi TCP read là message boundary. Giới hạn payload trước allocation; áp dụng timeout/cancellation; theo dõi số lần truncated frame và source connection, tránh log payload nhạy cảm.

## 9. Trade-offs
Đọc đủ byte giảm false rejects nhưng một peer có thể giữ connection mở mà không hoàn tất frame. Trong production cần giới hạn kích thước, deadline cho toàn frame, giới hạn số connection đồng thời và đóng kết nối khi protocol không hợp lệ.

## 10. What a Senior engineer should notice
Phân biệt **framing** (đủ byte), **transport** (chunk tùy ý) và **business parsing** (UTF-8/schema). Kiểm thử bằng fragmenting stream deterministic tốt hơn test phụ thuộc timing mạng; contract của `ReadAsync` quan trọng hơn kết quả thành công trên máy dev.

## Reference code
[solution/Program.cs](Program.cs) là một bản độc lập để đối chiếu; `verify.ps1` **không** chạy bản này.
