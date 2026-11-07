# Reference Solution — Spoiler

## 1. Symptoms
Batch đầu nhận GS-101. Sau khi worker khởi động lại, các batch tiếp theo trả về 0 bản ghi mặc dù feed vẫn có GS-102 đến GS-104.

## 2. Evidence
Fixture có bốn dòng và kích thước nhỏ hơn buffer đọc. Khi đọc dòng đầu, giá trị cursor của starter có thể đã gần hoặc bằng cuối file. Tại thời điểm đó, chỉ một bản ghi được xử lý ở tầng nghiệp vụ.

## 3. Root cause
`StreamReader` dùng buffering và có thể đọc trước nhiều byte vào bộ nhớ. `FileStream.Position` là vị trí byte đã được nạp từ stream nền, **không** phải offset của dòng cuối đã được `ReadLine()` trả về. Starter lưu vị trí này như checkpoint durable, rồi lần chạy sau `Seek` đến đó. Các dòng chưa xử lý nằm trong buffer cũ và bị bỏ qua khi process kết thúc.

## 4. Why the fix works
Bản reference lưu số **dòng logic đã xử lý**. Mỗi lần mở file, đọc bỏ đúng số dòng đó rồi xử lý tối đa `maxRecords` dòng tiếp theo. Đơn vị của cursor trùng với đơn vị tiến độ nghiệp vụ, độc lập với buffering của `StreamReader`.

## 5. How to verify
Sao chép `solution/FeedImporter.cs` đè lên `starter/FeedImporter.cs` trong bản làm việc, chạy `./verify.ps1`. Acceptance kiểm tra 1+2+1 sự kiện, EOF không replay, append GS-105 được nhận một lần. `./reproduce.ps1` lúc này phải không còn tái hiện triệu chứng cũ. Chưa có thực thi .NET trong lần phát hành này; validationMode = static-review.

## 6. Alternative fixes
- Byte-accurate line reader tự quản lý decoder, newline và offset: hỗ trợ resume O(1) nhưng cần xử lý UTF-8, CRLF, partial line và byte order mark cực kỳ cẩn thận.
- Durable event ID + unique constraint trong database: tốt hơn nếu cần chống trùng xuyên crash; chi phí lưu trữ và vận hành cao hơn.
- Replay toàn bộ file với dedup theo event ID: đơn giản cho feed nhỏ, tốn chi phí quét và cần định danh ổn định.

## 7. Wrong / tempting fixes
- Tăng buffer size: không giải quyết semantic của cursor, có thể làm read-ahead lớn hơn.
- Gọi `DiscardBufferedData()` sau `ReadLine()`: không tự khôi phục offset của dòng đã đọc; dễ làm mất dữ liệu đang buffer.
- Chỉ gọi `Flush()` trên FileStream: không thay đổi vị trí logic của StreamReader.
- Bắt exception và retry cùng cursor: không có exception, nên chỉ lặp lại trạng thái sai.

## 8. Production implications
Giải pháp line-count có chi phí khởi động lại O(n) và giả định file **append-only, không reorder, không truncate**. Cần lưu file identity/version và reject file replacement; nếu feed lớn, dùng byte-offset reader đúng nghĩa hoặc hệ thống event log. Để có exactly-once effects, phải gắn ghi kết quả nghiệp vụ và checkpoint trong cùng transaction hoặc có idempotent sink; việc ghi cursor sau side effect riêng lẻ vẫn có crash window. Checkpoint file cũng cần atomic replace và kiểm soát concurrent writers.

## 9. Trade-offs
Line-count checkpoint dễ audit và sửa lỗi nhưng resume chậm khi hàng triệu dòng. Byte-offset checkpoint nhanh nhưng yêu cầu parser tự quản lý boundary. DB-backed event ID phù hợp khi dữ liệu có giá trị cao và cần replay/retention.

## 10. Senior engineer should notice
Định nghĩa cursor bằng một invariant đo được; phân biệt bytes fetched, chars decoded, lines returned, events durably accepted. Thiết kế test restart sau batch ngắn, append, truncate, rotation, encoding và crash giữa side effect/checkpoint. Đừng đánh đồng pass trong fixture với exactly-once production guarantee.
