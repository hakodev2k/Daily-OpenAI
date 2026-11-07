# Scenario

`Ordering.Api` xử lý thao tác approve order theo flow hiện tại:

```text
HTTP request
  -> validate business rules
  -> update Orders.Status = Approved
  -> commit SQL transaction
  -> publish OrderApproved to broker
  -> return 200
```

## Constraints

- SQL Server là system of record cho order state.
- Inventory và Billing chỉ nhận thay đổi qua message broker.
- Broker có at-least-once delivery; duplicate delivery có thể xảy ra.
- Không có distributed transaction coordinator giữa SQL Server và broker.
- Request p95 target < 500 ms trong điều kiện bình thường.
- Team có 4 backend developers; không muốn bổ sung platform mới nếu không cần thiết.
- Mất một `OrderApproved` có thể khiến inventory không được reserve và invoice không được tạo.
- Duplicate `OrderApproved` cũng không được tạo duplicate reservation hoặc invoice.

## Current implementation

Pseudo-code hiện tại:

```csharp
await db.BeginTransactionAsync(ct);

order.Approve();
await db.SaveChangesAsync(ct);
await db.CommitTransactionAsync(ct);

await bus.PublishAsync(new OrderApproved(order.Id), ct);

return Results.Ok();
```

Bạn không được giả định rằng `PublishAsync` thành công chỉ vì database commit thành công.

## Decision required

Thiết kế một cơ chế làm cho state change và integration-event intent có thể recovery đáng tin cậy sau process crash, broker outage hoặc retry, mà không phụ thuộc vào distributed transaction giữa database và broker.
