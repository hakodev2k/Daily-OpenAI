# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Database xác nhận một row đã được chuyển sang `Approved`, nhưng object `order` đang được request giữ vẫn có `Status = Pending`. Query `AsNoTracking()` đọc lại database lại thấy `Approved`.

## Evidence

Starter cho ba evidence quan trọng: `AFFECTED=1`, `TRACKED=Pending`, `DATABASE=Approved`. Điều đó tách lỗi persist khỏi lỗi state trong memory.

## Root cause

`ExecuteUpdateAsync` thực hiện set-based update trực tiếp ở database và không cập nhật state của entity đã được EF Core change tracker giữ trước đó. Entity `order` vì vậy vẫn mang snapshot cũ trong cùng `DbContext`.

## Why the fix works

Reference solution gọi `db.Entry(order).ReloadAsync()` sau bulk update. EF Core đọc row hiện tại từ database và refresh tracked entity, nên response path dùng state đã persist.

## How to verify

Sửa `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Behavior cần đạt: `AFFECTED=1`, `TRACKED=Approved`, `DATABASE=Approved`, và process kết thúc thành công.

## Alternative fixes

- Tách bulk operation sang một `DbContext` riêng rồi query response bằng context mới.
- Không giữ tracked entity trước bulk operation nếu không cần nó.
- Sau bulk update, clear change tracker rồi query lại nếu boundary của use case cho phép và bạn hiểu tác động tới các entity khác đang track.
- Với use case nhỏ cần domain behavior trên entity, có thể dùng tracked update + `SaveChangesAsync` thay vì bulk update.

## Wrong or misleading fixes

- Gán thủ công `order.Status = "Approved"` mà không xác nhận database operation thành công: có thể khiến memory nói `Approved` trong khi persistence thất bại hoặc bị điều kiện lọc bỏ.
- Thêm `SaveChangesAsync()` sau `ExecuteUpdateAsync` như một nghi thức: bulk update đã chạy trực tiếp ở database; `SaveChangesAsync` không tự refresh tracked state.
- Tắt tracking toàn ứng dụng: có thể tránh symptom ở đường này nhưng thay đổi semantics rộng hơn mức cần thiết.

## Production implications

Mix set-based operations với tracked entities trong cùng unit of work có thể tạo state divergence khó nhìn thấy. Đặc biệt nguy hiểm khi response, domain event hoặc subsequent decision dùng entity đã load trước bulk operation.

## Trade-offs

`ReloadAsync()` rõ ràng và cục bộ nhưng thêm một round trip. Context separation tạo boundary sạch hơn nhưng tăng cấu trúc. Tracked update đơn giản hơn cho một số use case nhưng không phù hợp khi cần update nhiều rows hiệu quả.

## What a Senior engineer should notice

Câu hỏi quan trọng không phải chỉ là “database đúng chưa”, mà là “nguồn state nào đang được code tin tưởng sau một operation có semantics khác change tracker”. Hãy thiết kế transaction/context boundary để tránh tồn tại đồng thời hai nguồn sự thật không đồng bộ trong cùng request.
