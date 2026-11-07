# Reference Solution

Chỉ xem sau khi đã reproduce và thử fix.

## Symptoms
Operation đọc product thành công nhưng update bằng một object request riêng biệt thất bại trước khi lưu.

## Evidence
`ChangeTracker` đã chứa instance được query với key `1`. Request model là instance thứ hai có cùng key.

## Root cause
Một `DbContext` đang theo dõi entity đã query. Update path sau đó cố đưa một instance khác đại diện cho cùng identity vào cùng tracking scope.

## Reference fix
Giữ instance đã query làm aggregate state của operation và copy các giá trị được phép thay đổi từ request vào instance đó:

```csharp
current.Name = requestModel.Name;
current.Price = requestModel.Price;
await db.SaveChangesAsync();
```

## Why it works
EF Core tiếp tục quản lý một instance duy nhất cho identity đó và change detection ghi nhận các property thay đổi.

## Verification
Chạy `./verify.ps1`. Starter do learner chỉnh phải exit thành công và in `Update completed`.

## Alternative fixes
Một read path thực sự chỉ dùng để kiểm tra dữ liệu có thể dùng no-tracking rồi attach một update model, nhưng phải thiết kế rõ ownership và concurrency semantics.

## Wrong / tempting fixes
- `SaveChanges` sớm hơn không giải quyết ownership của tracked state.
- Tạo thêm Repository abstraction không tự thay đổi tracking semantics.
- Tạo `DbContext` mới chỉ để né symptom có thể che giấu transaction boundary và consistency requirement.

## Production implications
Update APIs cần quyết định rõ tracked-update hay disconnected-update. Với disconnected data, cần thêm validation, authorization và optimistic concurrency khi business correctness yêu cầu.

## Trade-offs
Tracked update đơn giản và an toàn cho operation ngắn. No-tracking + explicit attach hữu ích cho một số disconnected workflows nhưng yêu cầu quản lý state và concurrency cẩn thận hơn.

## Senior engineer should notice
Vấn đề không nằm ở `SaveChanges` mà ở state ownership trong một unit of work. Trước khi sửa, hãy xác định entity instance nào là source of truth của operation.