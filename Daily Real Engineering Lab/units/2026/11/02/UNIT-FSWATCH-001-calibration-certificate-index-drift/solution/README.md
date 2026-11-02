# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Cổng tra cứu giữ A ở phiên bản cũ, vẫn hiển thị C đã xóa, không thấy D/E mới. Maintenance counter tăng và không có exception.

## 2. Evidence

Trong Program.cs, thay đổi có notification (B cập nhật và B xóa) được phản ánh. Thay đổi không có notification không được phản ánh. CPU thấp và host còn sống không chứng minh tính đúng đắn của index.

## 3. Root cause

CertificateIndexer chỉ thay đổi chỉ mục qua OnNotification. Thiết kế ngầm coi notification là durable change log đầy đủ. FileSystemWatcher không bảo đảm exactly-once/durable delivery: event có thể bị gộp, trễ, bỏ sót hoặc buffer overflow. Harness giả lập thiếu event xác định. MaintenanceTick ban đầu không kiểm tra dữ liệu thực.

## 4. Why the fix works

[Code tham khảo](CertificateIndexer.cs) dùng trạng thái thư mục làm nguồn sự thật tại maintenance tick: quét .cert, cập nhật giá trị, xóa khóa không còn tồn tại. Tick lặp lại không làm thay đổi kết quả. Event vẫn là fast path; scan là recovery path.

## 5. How to verify

Chỉ sửa starter/CertificateIndexer.cs rồi chạy ./verify.ps1. Harness luôn chạy starter, không tự dùng code solution. Kiểm tra cả thêm/sửa/xóa, .tmp, tick lặp lại và pha thay đổi thứ hai.

## 6. Alternative fixes

- Durable change log/outbox nếu kiểm soát toàn bộ writer.
- Hybrid event-driven fast path và periodic reconciliation.
- Quét theo batch/hash/metadata để giảm I/O ở thư mục lớn.
- Nếu dữ liệu nằm trong database, cân nhắc CDC/change stream có recovery.

## 7. Wrong / tempting fixes

- Chỉ tăng buffer FileSystemWatcher: giảm overflow nhưng không bảo đảm delivery.
- Retry handler khi có exception: không giúp khi event không bao giờ tới.
- Restart worker mà không scan: không có bằng chứng sẽ sửa trạng thái.
- Chỉ upsert tệp tìm thấy: giữ mã chứng chỉ đã xóa.
- Coi File.Exists trước File.ReadAllText là atomic: writer có thể thay đổi file giữa hai bước.

## 8. Production implications

Harness single-threaded với tệp ổn định. Production cần snapshot/locking cho concurrent readers, xử lý tệp đang ghi dở (staging và atomic rename), retry lỗi đọc, giới hạn I/O, metrics độ lệch, và chính sách không xóa hàng loạt khi thư mục tạm không truy cập được.

## 9. Trade-offs

Full scan đơn giản nhưng tốn I/O ở quy mô lớn. Event-only có độ trễ thấp nhưng không đủ phục hồi. Hybrid tốn thêm chi phí nhưng kiểm soát được thời gian hội tụ. Durable log phù hợp khi có quyền kiểm soát nguồn ghi.

## 10. What a Senior engineer should notice

Phân biệt process liveness với data correctness và notification với authoritative state. Cần contract rõ cho consistency, deletion, replay, recovery, scale, partial failures và observability.

Tài liệu chính thức: [FileSystemWatcher](https://learn.microsoft.com/dotnet/api/system.io.filesystemwatcher), [Directory.EnumerateFiles](https://learn.microsoft.com/dotnet/api/system.io.directory.enumeratefiles).
