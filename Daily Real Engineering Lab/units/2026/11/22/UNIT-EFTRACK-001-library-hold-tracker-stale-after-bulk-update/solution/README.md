# Reference solution — đọc lại sau set-based update

`ExecuteUpdate` thực thi SQL ngay, bỏ qua change tracker và không cập nhật entity `hold` đã được materialize. Một tracking query theo cùng key có thể trả về instance đang theo dõi với `Status=Pending`, mặc dù database đã có `Approved`. `SaveChanges` không đồng bộ hóa tự động kết quả của `ExecuteUpdate`.

Giải pháp mẫu: lấy projection hoặc entity với `AsNoTracking()` sau khi cập nhật và dùng chính kết quả đó để tạo response. Trong ứng dụng thực, nên dùng DTO projection, tách rõ read-model sau write. Nếu cần tiếp tục sửa tracked entity, cân nhắc `Entry(hold).Reload()` (phát sinh query) hoặc `ChangeTracker.Clear()` (ảnh hưởng mọi entity đang track); không áp dụng bừa bãi. `ExecuteUpdate` không tự động áp dụng concurrency token trong bộ nhớ: với tranh chấp đồng thời phải thêm điều kiện version vào WHERE và kiểm tra affected rows.

## Wrong fixes
- Gọi `SaveChanges()` ngay sau `ExecuteUpdate`: không tự refresh tracked entity, thậm chí có nguy cơ ghi đè thay đổi nếu entity bị đánh dấu Modified.
- Thêm `Task.Delay`: database đã được cập nhật đồng bộ, chờ không thay đổi identity map.
- Chuyển toàn bộ query sang tracking mặc định: tái sử dụng state cũ.
- `ChangeTracker.Clear()` toàn cục trong request nhiều nghiệp vụ: có thể bỏ các thay đổi chưa lưu.

## Verify
Chạy `./scripts/verify.ps1` sau khi sửa starter; output phải có `AFFECTED=1`, `DATABASE=Approved`, `RESPONSE=Approved`. Không chấp nhận sửa chỉ output `DATABASE` hoặc script kiểm tra.
