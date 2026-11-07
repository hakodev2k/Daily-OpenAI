# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Hai delivery có cùng `EventId` và `OrderId`, consumer chạy thành công cả hai lần, và shipping gateway nhận hai reservation request cho cùng order.

## 2. Evidence

Các dòng `Delivery EventId=...`, `DeliveryCount=2` và `ReservationsForOrder=2` cho thấy transport delivery count và business side-effect count đang bị gắn trực tiếp với nhau.

## 3. Root cause

Consumer không có idempotency boundary. Mỗi lần `HandleAsync` được gọi đều thực hiện side effect mới, kể cả khi logical event đã được xử lý trước đó.

At-least-once delivery là behavior bình thường của nhiều messaging systems khi ack bị mất, consumer restart hoặc retry/redelivery xảy ra. Duplicate delivery vì thế phải được xem là một failure mode hợp lệ của transport.

## 4. Why the fix works

Trong lab local, có thể dùng processed-event store đơn giản để minh họa cơ chế:

```csharp
public sealed class ShippingConsumer(FakeShippingGateway shipping)
{
    private readonly HashSet<string> _processedEventIds = new(StringComparer.Ordinal);

    public async Task HandleAsync(OrderPaidEvent message)
    {
        if (!_processedEventIds.Add(message.EventId))
            return;

        await shipping.ReserveAsync(message.OrderId);
    }
}
```

Cùng `EventId` chỉ được phép đi qua business side-effect boundary một lần.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Expected:

- `DeliveryCount=2`
- `ReservationsForOrder=1`
- script kết thúc với `PASS`

Sau đó thử thêm một event mới với `EventId` khác để chắc chắn consumer không chặn legitimate work.

## 6. Alternative fixes

Trong production, in-memory `HashSet` không đủ. Các lựa chọn thực tế gồm:

- Inbox table với unique constraint trên `MessageId/EventId`.
- Idempotency record trong cùng transactional database với business state.
- Downstream API hỗ trợ idempotency key và enforce uniqueness.
- Business invariant tự nhiên, ví dụ unique shipment reservation per order, nếu invariant đó đúng với domain.

## 7. Wrong or misleading fixes

### Tắt retry hoặc yêu cầu broker chỉ delivery một lần

Không loại bỏ được mọi duplicate delivery. Network acknowledgement loss và consumer restart vẫn có thể tạo redelivery.

### Giữ `HashSet` static/in-memory trong production

Chỉ hoạt động trong một process lifetime. Restart hoặc scale-out sẽ mất/shared không đồng nhất state.

### Check downstream trước rồi mới create

Một `check-then-act` không atomic có race condition khi hai consumer xử lý song song.

### Dùng distributed lock cho mọi message

Có thể hữu ích trong một số topology nhưng thường phức tạp hơn unique constraint hoặc transactional inbox; lock cũng không tự giải quyết crash giữa side effect và ack.

## 8. Production implications

Idempotency cần được thiết kế cùng transaction boundary. Nếu processed marker được ghi trước side effect rồi process crash, message có thể bị bỏ qua dù side effect chưa xảy ra. Nếu marker ghi sau side effect rồi process crash trước marker, redelivery có thể tạo side effect lần hai.

Do đó production design thường cần atomic persistence giữa business state và inbox record, hoặc downstream idempotency contract đủ mạnh.

## 9. Trade-offs

- Inbox table: rõ ràng, durable, dễ audit nhưng tăng storage và cleanup responsibility.
- Business unique constraint: rất mạnh và đơn giản nếu domain invariant phù hợp, nhưng không áp dụng cho mọi side effect.
- Downstream idempotency key: tốt khi downstream hỗ trợ contract này, nhưng chuyển một phần responsibility sang dependency.
- Distributed lock: coordination cost cao hơn và failure semantics phức tạp.

## 10. What a Senior engineer should notice

Senior engineer không chỉ sửa duplicate hiện tại mà phải hỏi:

- Transport delivery semantics là gì?
- Idempotency key nào thực sự đại diện cho logical operation?
- Dedup state có durable và shared giữa instances không?
- Marker và business side effect có atomic không?
- Retention của idempotency records bao lâu?
- Một event mới hợp lệ có thể dùng lại business key cũ hay không?
- Nếu downstream timeout sau khi đã commit side effect thì consumer xác định kết quả thế nào?

Điểm cốt lõi là tách **message delivery count** khỏi **business effect count**.
