# Incident Timeline

Thời gian tương đối của một request `ApproveOrder(8142)`:

```text
T+000 ms  Request bắt đầu.
T+061 ms  Business validation pass.
T+094 ms  SQL UPDATE Orders SET Status='Approved' WHERE Id=8142.
T+109 ms  SQL transaction COMMIT thành công.
T+112 ms  Process bị terminate do node restart.
T+2.8 s   Instance mới healthy.
T+9.4 s   Support query thấy Order 8142 = Approved.
T+15 s    Không có OrderApproved(8142) trong broker telemetry.
T+4 min   Inventory vẫn chưa reserve hàng.
```

## Additional evidence

- Không có exception log từ `PublishAsync` vì process chết trước khi lời gọi bắt đầu.
- SQL transaction log xác nhận commit thành công.
- Broker telemetry không có message ID liên quan đến order 8142.
- Client đã mất connection và retry request sau 3 giây.
- Retry nhận response `409 AlreadyApproved` và không publish event.

## Investigation questions

- Failure window nằm giữa hai durability boundary nào?
- Retry HTTP có đủ để recovery business intent không?
- Nếu publisher retry message, downstream phải bảo vệ điều gì?
- Bạn cần quan sát metric/state nào để phát hiện event bị kẹt trước khi người dùng báo lỗi?
