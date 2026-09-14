# Reference Solution

> Chỉ xem sau khi đã reproduce và thử fix.

## Symptoms

Endpoint cần chỉ trả về `Open` orders nhưng object graph cuối cùng có cả `Closed` order sau khi cùng `DbContext` đã tải full history.

## Evidence

Audit query materialize toàn bộ orders và giữ chúng trong tracking state. Filtered `Include` ở query sau không tạo ra một isolation boundary cho object graph đang được tracking.

## Root cause

EF Core navigation fixup liên kết các entity đã được tracking với navigation tương ứng. Với tracking query, những `Order` đã tồn tại trong context có thể xuất hiện lại trong `Customer.Orders` dù filtered `Include` của query sau chỉ yêu cầu `Open` orders.

## Why the fix works

Với read-model endpoint này, dùng `AsNoTracking()` trước filtered `Include` tạo object graph riêng không tham gia tracking state đã có từ audit query. Một lựa chọn còn rõ contract hơn là project trực tiếp sang DTO và filter collection trong projection.

Ví dụ tối thiểu:

```csharp
var customer = await db.Customers
    .AsNoTracking()
    .Include(c => c.Orders.Where(o => o.Status == OrderStatus.Open))
    .SingleAsync(c => c.Id == 1);
```

## How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Pass condition: collection endpoint chỉ còn `Open` orders dù audit query vẫn chạy trước đó.

## Alternative fixes

- Projection sang DTO/read model với `Select` và filter rõ ràng.
- Dùng `DbContext` riêng cho read operation nếu transaction/lifetime boundary của application phù hợp.
- Clear tracking state chỉ khi đó thực sự là boundary hợp lệ; không dùng như workaround mù quáng.

## Wrong / tempting fixes

- Filter lại collection trong memory sau khi query nhưng vẫn giữ API contract phụ thuộc implicit tracking state: che triệu chứng hơn là làm boundary rõ ràng.
- Bỏ audit query chỉ để test pass: thay đổi requirement thay vì sửa failure mechanism.
- Gọi `ChangeTracker.Clear()` ở mọi nơi: có thể phá unit-of-work semantics và che thiết kế lifetime chưa rõ.

## Production implications

Tracking state là stateful behavior trong `DbContext`. Read endpoints dài hoặc service method kết hợp nhiều query dễ tạo coupling khó thấy giữa thứ tự query và object graph trả về.

## Trade-offs

`AsNoTracking()` phù hợp read-only flow và giảm tracking overhead, nhưng không phù hợp nếu entity graph đó cần được chỉnh sửa rồi `SaveChanges()` trong cùng unit of work. Projection thường rõ contract hơn nhưng cần mapping/read-model code.

## What a Senior engineer should notice

- SQL result và in-memory entity graph không phải lúc nào cũng là cùng một abstraction.
- `DbContext` lifetime quyết định phạm vi identity map/tracking state.
- Query order không nên âm thầm làm thay đổi API response contract.
- Read model nên có boundary rõ ràng thay vì phụ thuộc vào incidental tracked entities.
