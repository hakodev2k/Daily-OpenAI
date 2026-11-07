# Reference Solution — UNIT-EFBULK-001

**Root cause:** `ExecuteUpdateAsync` thực thi SQL trực tiếp, không đồng bộ giá trị hoặc original snapshot của entity đang được track. Truy vấn tracking trả lại entity đã nằm trong identity map; do đó response lấy giá cũ dù SQL đã cập nhật.

## Một cách sửa có thể bảo vệ
Xem `TicketPricingService.cs`. Sau khi xác nhận affected rows, gọi `db.Entry(ticket).ReloadAsync()` cho **đúng entity** rồi mới xây response. Không gọi `ChangeTracker.Clear()` toàn cục vì sẽ bỏ theo dõi pending edit của vé khác. `ReloadAsync` cũng ghi đè pending changes trên chính entity được reload, nên cần tách command context hoặc giải quyết xung đột nếu nghiệp vụ cho phép chỉnh cùng entity.

## So sánh các phương án
- Targeted reload: ít thay đổi, thêm một SELECT; cần cân nhắc transaction/isolation nếu writer khác cập nhật giữa UPDATE và SELECT.
- Không track entity trước bulk update, rồi query `AsNoTracking`: đơn giản hơn khi thiết kế command/query tách biệt.
- Trả về giá yêu cầu trực tiếp: tránh SELECT nhưng không phản ánh trigger/normalization do database thực thi.
- `ChangeTracker.Clear()`: quá rộng, có thể làm mất pending edits và làm hỏng Unit of Work.

## Wrong fixes
- `SaveChangesAsync()` sau bulk update: không tự đồng bộ giá của entity đã track.
- Chỉ đổi `AsNoTracking()` ở một query không thực sự được dùng để tạo response: symptom vẫn tồn tại.
- Gọi `ChangeTracker.Clear()` toàn cục: có thể pass kiểm tra giá nhưng fail invariant pending edit.

## Tự kiểm tra
Sao chép nội dung `solution/TicketPricingService.cs` sang `starter/TicketPricingService.cs` sau khi đã tự làm; chạy `./verify.ps1`. Chạy `./reproduce.ps1` trước khi sửa hoặc sau `./reset.ps1`.

## Giới hạn
SQLite in-memory dùng để kiểm chứng tracking và affected rows, không mô phỏng mọi isolation level của SQL Server/PostgreSQL. Trong production cần xử lý concurrency token và transaction nếu giá có thể bị ghi đè đồng thời.
