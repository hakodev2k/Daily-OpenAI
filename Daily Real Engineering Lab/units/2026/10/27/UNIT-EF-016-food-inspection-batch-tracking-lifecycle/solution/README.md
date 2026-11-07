# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Import biên bản thành công, nhưng bước duyệt sau đó trả về `SaveChangesRows=0`; entity đang tracked có `Approved` trong khi SQLite vẫn là `Pending`.

## 2. Evidence

Chạy `./reproduce.ps1`. So sánh `AutoDetectChangesEnabled` trước/sau import, `SaveChangesRows`, `Tracked` và `Persisted` sau `ChangeTracker.Clear()`. Test exception path kiểm tra cùng cấu hình khi import dừng giữa chừng.

## 3. Root cause

`InspectionImportService.Import` thay đổi `DbContext.ChangeTracker.AutoDetectChangesEnabled` thành `false` nhưng không khôi phục. DbContext được tái sử dụng cho bước duyệt. EF Core không tự phát hiện thay đổi scalar property của entity ở trạng thái `Unchanged`, nên `SaveChanges` không phát UPDATE. Các entity được `Add` vẫn có state `Added`, vì vậy import thành công làm lỗi khó nhận ra.

## 4. Why the fix works

Lưu giá trị ban đầu, tạm thời tắt auto detection trong vùng import, và khôi phục bằng `finally` ở mọi exit path. Không gán cứng `true` vì caller có thể chủ động cấu hình `false`. Xem [InspectionImportService.cs](InspectionImportService.cs); chép file này vào `starter/` rồi chạy `./verify.ps1` để kiểm tra.

## 5. How to verify

`./verify.ps1` chạy chính project starter đã sửa, xác nhận: (1) trạng thái duyệt thực sự lưu; (2) exception không làm rò rỉ cấu hình; (3) trạng thái cấu hình ban đầu `false` được bảo toàn.

## 6. Alternative fixes

Có thể không tắt `AutoDetectChangesEnabled` nếu batch nhỏ và profiling không chứng minh lợi ích. Có thể dùng context ngắn hạn riêng cho import, rồi tạo context mới cho bước duyệt; cách này cải thiện isolation nhưng không thay thế việc giữ đúng hợp đồng khi thay đổi cấu hình context.

## 7. Wrong / tempting fixes

- Gọi `ChangeTracker.DetectChanges()` ở màn hình duyệt có thể làm test cụ thể pass nhưng giữ lại trạng thái context bị thay đổi cho mọi thao tác sau.
- Gán `AutoDetectChangesEnabled = true` sau import không bảo toàn cấu hình caller và không xử lý exception nếu không có `finally`.
- Gọi `Update(entity)` ép ghi có thể che giấu nguyên nhân, đồng thời ghi nhiều trường không mong muốn.
- Tăng timeout, retry hoặc đổi SQLite thành SQL Server không giải quyết lỗi state machine của EF Core.

## 8. Production implications

Phạm vi `DbContext` nên tương ứng unit of work; sau exception, discard context thay vì tiếp tục commit các entity `Added` chưa hoàn tất. Quan sát rows affected và kiểm tra persisted state ở workflow quan trọng. Không chia sẻ DbContext giữa các thread.

## 9. Trade-offs

Tắt auto detection có thể hữu ích trong batch lớn nhưng tăng nghĩa vụ quản lý lifecycle. Với dữ liệu ít, tính đơn giản và correctness thường có giá trị hơn tối ưu chưa đo. Nếu chuyển sang bulk SQL / ExecuteUpdate, cần đánh giá thêm tracking consistency và transaction boundaries.

## 10. Senior engineer should notice

Một flag tối ưu là mutable state có phạm vi sống dài hơn một method; review cần kiểm tra success, exception, caller-owned configuration và transaction semantics. `SaveChanges() == 0` không đồng nghĩa thao tác nghiệp vụ thành công.
