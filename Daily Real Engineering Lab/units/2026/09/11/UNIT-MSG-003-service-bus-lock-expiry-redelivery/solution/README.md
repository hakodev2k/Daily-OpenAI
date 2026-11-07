# Reference Solution — UNIT-MSG-003

> Chỉ xem sau khi đã reproduce và thử sửa trong `starter/`.

## 1. Symptoms

Worker nhận một message nhưng khi processing kéo dài, cùng `messageId` xuất hiện ở delivery khác trước khi delivery đầu hoàn tất. Nhiều handler vì thế có thể tạo cùng output.

## 2. Evidence

Starter cho thấy `LOCK_EXPIRED`, `DELIVERIES > 1`, `MAX_CONCURRENT_FOR_MESSAGE > 1` và nhiều `OUTPUT_WRITTEN` cho cùng message.

## 3. Root cause

Processing duration lớn hơn message lock duration và handler không duy trì lock trong khi công việc còn chạy. Broker vì thế coi message đủ điều kiện redelivery dù handler đầu chưa kết thúc.

## 4. Why the fix works

Reference solution bắt đầu một renewal loop khi handler nhận ownership. Loop gia hạn lock theo chu kỳ ngắn hơn lock duration và dừng sau khi side effect + completion hoàn tất. Trong simulator, gọi `Delivery.RenewLock()`; với Azure Service Bus thực tế, cùng contract được hỗ trợ qua lock renewal/auto lock renewal của SDK.

## 5. How to verify

Áp dụng logic trong `solution/PreviewWorker.cs` vào `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kỳ vọng `DELIVERIES=1`, `MAX_CONCURRENT_FOR_MESSAGE=1`, `SIDE_EFFECTS=1`, `COMPLETIONS=1`.

## 6. Alternative fixes

- Tăng broker lock duration có thể hợp lý khi thời gian xử lý có upper bound rõ và nằm trong giới hạn cấu hình.
- Tách công việc dài thành các bước/message nhỏ hơn nếu một handler giữ lock quá lâu.
- Với `ServiceBusProcessor`, cấu hình auto lock renewal phù hợp với worst-case processing time.
- Thiết kế side effect idempotent vẫn rất quan trọng vì at-least-once delivery có thể xảy ra vì nhiều nguyên nhân khác ngoài lock expiry.

## 7. Wrong or misleading fixes

- Tăng số consumer: làm tăng khả năng duplicate processing, không sửa ownership contract.
- Retry handler ngay khi thấy lỗi completion: có thể nhân side effect nếu công việc đã thực hiện xong.
- Chỉ deduplicate bằng in-memory flag: không bảo vệ khi scale-out hoặc process restart.
- Đặt lock duration cực lớn không giới hạn: che giấu workload bất thường và kéo dài thời gian recovery khi consumer chết.

## 8. Production implications

Theo dõi processing duration, lock-lost exceptions, delivery count và duplicate business side effects. `MaxAutoLockRenewalDuration`/renewal policy phải khớp workload thực tế và cancellation/shutdown behavior.

## 9. Trade-offs

Renewal giữ ownership lâu hơn nên làm chậm redelivery khi worker bị treo nếu cancellation/failure handling không tốt. Chia nhỏ message tăng orchestration complexity nhưng giảm thời gian giữ lock. Idempotency tăng độ an toàn nhưng thường cần durable state và transaction boundary rõ.

## 10. What a Senior engineer should notice

Lock renewal giải quyết ownership window, không biến queue thành exactly-once system. Thiết kế production vẫn phải giả định redelivery có thể xảy ra và tách rõ: broker settlement, business side effect, idempotency và observability.
