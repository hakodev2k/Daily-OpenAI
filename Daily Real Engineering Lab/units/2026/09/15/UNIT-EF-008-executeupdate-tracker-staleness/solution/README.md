# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Database đã tăng giá từ `100.00` lên `110.00`, nhưng business rule vẫn đọc `100.00` từ object đã load trước đó nên không phát alert.

## Evidence

Starter in đồng thời hai góc nhìn:

- query `AsNoTracking()` mới từ database trả `110.00`
- entity `product` đã được load trước bulk update vẫn có `100.00`

## Root cause

`ExecuteUpdate` là set-based operation chạy trực tiếp ở database. Nó không cập nhật state của entity instance đã nằm trong EF Core change tracker. Vì vậy cùng một `DbContext` có thể chứa tracked state cũ trong khi database đã thay đổi.

## Why the fix works

Reference solution gọi:

```csharp
await db.Entry(product).ReloadAsync();
```

sau bulk update và trước khi business rule đọc lại entity. `ReloadAsync` lấy state hiện tại từ database vào tracked instance, nên business rule và database cùng quan sát `110.00`.

## How to verify

Áp dụng fix tương đương trong `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Verification chỉ pass khi:

- database price = `110.00`
- business-rule price = `110.00`
- alert = `True`

## Alternative fixes

- Không giữ entity tracked qua boundary của set-based update; query lại dữ liệu cần dùng sau update.
- Dùng `ChangeTracker.Clear()` rồi query lại nếu workflow thực sự muốn bỏ toàn bộ tracked graph hiện tại.
- Tổ chức lại unit of work để set-based write và tracked workflow không chia sẻ cùng một context khi chúng không cần thiết phải chia sẻ state.

Lựa chọn phù hợp phụ thuộc số entity đang tracked, transaction boundary và việc context còn chứa pending changes hay không.

## Wrong / tempting fixes

- Gán tay `product.Price += 10`: có thể làm demo pass nhưng duplicate business logic và dễ sai khi update expression phức tạp hơn.
- Bỏ `AsNoTracking()` ở query kiểm tra: không giải quyết stale tracked instance; thậm chí query tracking có thể trả object đã tracked và che mất bằng chứng database đã đổi.
- Tạo `DbContext` sống lâu hơn: làm stale-state risk lớn hơn, không nhỏ hơn.

## Production implications

Khi trộn `ExecuteUpdate`/`ExecuteDelete` với tracked entities trong cùng context, cần xác định rõ state nào là authoritative sau set-based operation. Sai assumption có thể gây decision sai, response cũ, hoặc overwrite ngoài ý muốn ở các bước tiếp theo.

## Trade-offs

`ReloadAsync` rất rõ ràng cho một entity nhưng tạo thêm round trip. `ChangeTracker.Clear()` rẻ về mặt quản lý state nhưng phá toàn bộ tracking hiện tại. Query bằng context mới tạo boundary sạch hơn nhưng tăng lifecycle complexity. Senior engineer nên chọn boundary đơn giản nhất phù hợp workflow.

## What a Senior engineer should notice

Vấn đề không nằm ở SQL update có thất bại hay không. Database đã đúng. Failure xuất hiện vì application có hai mô hình state cùng tồn tại: database state và tracked in-memory state. Khi dùng set-based APIs, phải thiết kế rõ điểm reconcile giữa hai mô hình này.
