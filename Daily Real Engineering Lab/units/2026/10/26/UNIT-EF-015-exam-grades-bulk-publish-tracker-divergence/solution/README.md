# Reference Solution — chỉ xem sau khi reproduce và tự sửa

## 1. Symptoms
Batch publication cập nhật đúng một hàng; sau bước bổ sung ghi chú, trạng thái cuối trong database lại là Draft dù không có exception.

## 2. Evidence
Trước batch, entity trong DbContext có Status=Draft. ExecuteUpdateAsync báo 1 affected row, database đã là Published. Tracked instance vẫn giữ Draft. Khi SaveChanges chạy, câu lệnh cập nhật từ tracked instance mang giá trị Draft xuống storage. Harness đọc lại từ SQLite và thấy Draft.

## 3. Root cause
ExecuteUpdateAsync chạy set-based SQL ngay lập tức và **không đồng bộ EF Core change tracker**. Sau đó DbSet.Update(grade) đánh dấu các scalar properties là Modified; SaveChanges ghi đè cả Status bằng giá trị cũ từ tracked instance. Không phải do SQLite transaction bị rollback.

## 4. Why the fix works
Sau ExecuteUpdateAsync, gọi **await db.Entry(grade).ReloadAsync()** để refresh giá trị persisted vào tracked entity. Chỉ thay đổi ReviewerNote rồi SaveChangesAsync, không gọi Update trên entity đã tracked. Status=Published được giữ nguyên.

## 5. How to verify
Sửa **starter/GradesService.cs** theo **solution/GradesService.cs** (không sửa harness), chạy **./verify.ps1**. Kỳ vọng Status=Published, Score=92, ReviewerNote=QA-reviewed, exit code 0. Verify kiểm tra lại giá trị đọc từ database sau ChangeTracker.Clear.

## 6. Alternative fixes
Có thể clear tracker rồi query lại entity sau bulk update; hoặc thiết kế note update riêng bằng ExecuteUpdate chỉ tác động ReviewerNote. Với nhiều request, cân nhắc dùng DbContext ngắn hạn tách batch và chỉnh sửa; phải kiểm tra optimistic concurrency khi nhiều người cùng sửa.

## 7. Wrong or misleading fixes
- Retry SaveChanges không thay đổi state cũ và có thể tiếp tục ghi đè.
- Chỉ thêm AsNoTracking vào lần đọc cuối chỉ làm lộ symptom, không sửa dữ liệu.
- Gọi Update nhiều lần vẫn đánh dấu toàn entity; không nên xem Update là yêu cầu bắt buộc cho entity đã tracked.
- Bỏ kiểm tra Status trong test chỉ che giấu data loss.

## 8. Production implications
Nếu batch và sửa ghi chú thuộc hai use case khác nhau, thiết kế boundary rõ ràng. Ghi log row count và correlation ID; bổ sung regression test sau deploy. Nếu đồng thời có writer khác, refresh đơn thuần chưa bảo vệ khỏi lost update: dùng concurrency token hoặc conditional update theo business rule.

## 9. Trade-offs
ReloadAsync thêm một round trip; clear/requery cũng vậy nhưng dễ làm mất pending changes khác. Set-based note update tiết kiệm round trip nhưng cần thiết kế điều kiện cập nhật và concurrency phù hợp. Tránh dùng blanket Update cho tracked entity nếu chỉ muốn đổi một field.

## 10. Senior engineer should notice
Phân biệt database state, tracked state, entity state flags và SQL actually emitted. Một row affected của bulk update không chứng minh dữ liệu còn đúng sau các bước kế tiếp. Test phải đọc lại persisted state qua boundary độc lập, không chỉ assert object in-memory.

## Reference source
[GradesService.cs](GradesService.cs). [Tài liệu chính thức](../docs/references.md).
