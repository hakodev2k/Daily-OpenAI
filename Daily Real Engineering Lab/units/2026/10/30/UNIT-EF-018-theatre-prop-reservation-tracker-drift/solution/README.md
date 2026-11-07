# Reference Solution — chỉ đọc sau khi tự thử

## 1. Symptoms
Hai thao tác đều báo thành công nhưng số lượng persisted bị lệch; một giao dịch vượt quá lượng còn lại vẫn được chấp nhận.

## 2. Evidence
Tại fixture (10,2,1), database sau stage allocation có 8 nhưng object quầy giữ vẫn có 10. Sau khi quầy giảm 1 rồi `SaveChangesAsync`, database thành 9 thay vì 7. Fixture (3,2,2) minh họa việc quyết định từ dữ liệu cũ.

## 3. Root cause
`ExecuteUpdateAsync` phát lệnh SQL trực tiếp, **không cập nhật entity đã tracked** trong `DbContext`. `SaveChangesAsync` tiếp theo dùng giá trị đã bị stale trong tracked object và ghi đè cập nhật trước đó.

## 4. Why the fix works
Trong [Program.cs](Program.cs), sau khi stage allocation hoàn tất, gọi `await db.Entry(tracked).ReloadAsync();` trước khi tính số lượng thuê. Giá trị in-memory được đồng bộ với persisted state, vì vậy cả kiểm tra đủ hàng và phép trừ tiếp theo dùng snapshot mới.

## 5. How to verify
Áp dụng cùng thay đổi vào `starter/Program.cs` rồi chạy `./verify.ps1` từ root unit. Phải có ba PASS. `reproduce.ps1` chỉ dành cho starter gốc, không phải test hồi quy sau sửa.

## 6. Alternative fixes
- Dùng set-based conditional update cho **cả** nghiệp vụ, ví dụ `WHERE Available >= requested`, trừ trực tiếp trên database và kiểm tra số row affected. Cách này phù hợp hơn với cạnh tranh liên request.
- Dùng optimistic concurrency token và retry có kiểm soát, đọc lại trạng thái và đánh giá lại điều kiện kinh doanh khi conflict.
- Tách scope `DbContext` giữa các bước có thể giúp đọc lại dữ liệu nhưng không tự tạo atomicity.

## 7. Wrong / tempting fixes
- Gọi `ChangeTracker.Clear()` rồi tiếp tục dùng reference cũ: object cũ vẫn stale và đã detached.
- Thêm `AsNoTracking()` vào query cuối: chỉ làm kết quả quan sát chính xác hơn, không sửa logic trước đó.
- Bỏ validation số lượng để tránh exception: che lỗi nghiệp vụ.
- Chỉ thêm transaction quanh logic hiện tại: transaction không tự đồng bộ tracked entity.

## 8. Production implications
Nếu nhiều request có thể cập nhật cùng hàng tồn kho, reload-then-save vẫn có cửa sổ race. Cần atomic conditional update hoặc concurrency token, kiểm tra affected rows và xử lý conflict theo quy tắc nghiệp vụ. Với inventory quan trọng, xem xét audit trail và transaction boundary.

## 9. Trade-offs
Reload thêm một round trip và dễ áp dụng trong refactoring hẹp. Atomic SQL update giảm race và thường ít round trip hơn, nhưng đòi hỏi thiết kế rõ ràng về business invariants và xử lý rejected rows. Optimistic concurrency phù hợp khi nhiều trường và aggregate rules phải được kiểm soát.

## 10. What a Senior engineer should notice
Phải phân biệt state trong ChangeTracker với state trong database; xác định đường ghi nào đi qua change tracking, đường nào bỏ qua; kiểm tra cả quyết định nghiệp vụ và persisted result; không tuyên bố concurrency-safe chỉ vì ba test tuần tự xanh.

## Official references
- [EF Core ExecuteUpdate / ExecuteDelete](https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete)
- [EF Core Change Tracking](https://learn.microsoft.com/en-us/ef/core/change-tracking/)
