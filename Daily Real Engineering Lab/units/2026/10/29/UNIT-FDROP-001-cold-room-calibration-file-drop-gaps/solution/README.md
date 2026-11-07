# Reference Solution — chỉ đọc sau khi đã tự thử

## Symptoms
Ba tệp JSON hoàn chỉnh hiện diện nhưng chỉ hai chứng nhận vào sổ; không có exception. Chạy kiểm tra lần hai còn bỏ sót chứng nhận mới nếu không có callback.

## Evidence
`ON_DISK` có `cal-102.json`; `ACCEPTED` thiếu `CAL-102`. Callback cho `cal-103.json` xuất hiện hai lần nhưng không làm tăng bản ghi. `Reconcile()` được gọi nhưng không thay đổi trạng thái.

## Root cause
Dịch vụ dùng các notification kiểu `FileSystemWatcher.Created` như nguồn công việc đầy đủ. Event filesystem không phải durable queue: event có thể bị mất khi buffer overflow, restart, hoặc thời điểm đăng ký watcher. Fixture cố ý mô phỏng việc không nhận một callback. Hook đối soát cuối ca chưa lấy lại các file đang tồn tại.

## Why the fix works
[CertificateIntake.cs](CertificateIntake.cs) duyệt inventory thư mục ở `Reconcile()`, tái sử dụng cổng `OnCreated()` để validate và tiếp nhận từng chứng nhận. `HashSet` theo `CertificateId` làm đối soát/callback lặp idempotent trong một process. Việc đối soát khôi phục các file hoàn chỉnh mà callback chưa từng được phát.

## How to verify
Trước khi tham khảo, hãy sửa `starter/CertificateIntake.cs` bằng giải pháp của bạn và chạy `./verify.ps1`. Để đối chiếu reference, có thể sao chép nội dung file reference sang starter sau khi lưu bài làm riêng. Không chỉnh `starter/Program.cs` hoặc fixture để làm test pass.

## Alternative fixes
- Periodic scan + watcher: watcher tối ưu latency, scan bảo đảm eventual reconciliation.
- Durable ingestion queue: producer ghi metadata công việc vào queue/DB có acknowledgement; phù hợp nếu kiểm soát được producer.
- Atomic rename từ `.tmp` sang `.json`: giảm nguy cơ đọc file chưa hoàn tất, nhưng không thay thế inventory reconciliation.

## Wrong / tempting fixes
- Tăng `InternalBufferSize` của watcher: có thể giảm mất event do overflow, không giải quyết restart và không tạo durable history.
- Chỉ retry callback thất bại: không thể retry callback chưa từng nhận.
- Hard-code ID bị thiếu: không mở rộng cho file khác.
- Chỉ quét một lần lúc startup: không xử lý được event bị mất sau startup.

## Production implications
Demo chỉ kiểm tra một process và file đã hoàn tất. Với production: dùng rename/marker hoàn tất, xử lý transient IOException/JSON chưa hoàn chỉnh, checkpoint bền vững hoặc unique constraint theo business ID, transaction/outbox nếu cần side effect, quarantine file lỗi, metric `pendingFiles` và `lastSuccessfulReconcileAt`, giới hạn batch/scan interval. Multi-worker cần uniqueness tại authoritative storage thay vì chỉ `HashSet` trong RAM.

## Trade-offs
Scan định kỳ tăng disk I/O và độ trễ phát hiện tối đa bằng chu kỳ scan; watcher giảm latency nhưng không thay thế kiểm chứng đầy đủ. DB checkpoint thêm chi phí vận hành song bảo đảm restart-safe. Cần phân biệt at-least-once delivery và exactly-once business effect.

## What a Senior engineer should notice
Một tín hiệu không durable không thể tự chứng minh sự đầy đủ. Định nghĩa invariant dựa trên trạng thái nguồn, làm đường xử lý idempotent, đo độ trễ đối soát và kiểm thử missing/duplicate/restart trước khi vận hành.
