# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Consumer có thể kết thúc một delivery nhưng business projection tương ứng không tồn tại. Lần xử lý sau cũng không nhận lại work item đó.

## 2. Evidence

Starter cho thấy sau failure point: `messageCompleted=true` trong khi `projectionExists=false`. Đây là hai state transitions bị lệch nhau.

## 3. Root cause

Consumer gọi acknowledgement (`Complete`) trước khi durable business work (`ProjectionStore.Save`) hoàn tất. Nếu process dừng trong khoảng giữa hai operations, broker coi message đã xử lý xong và không redeliver, trong khi business state chưa được ghi.

## 4. Why the fix works

Thực hiện durable work trước, chỉ acknowledgement sau khi work thành công. Khi failure xảy ra trước commit/Save, message vẫn chưa completed nên lần delivery kế tiếp còn cơ hội xử lý lại.

## 5. How to verify

Sau khi áp dụng fix vào `starter/`:

```powershell
./verify.ps1
```

Cả `happyPath` và `crashRecovery` phải là `True`.

## 6. Alternative fixes

Trong broker thực tế, có thể dùng explicit settlement, transaction support hoặc inbox/idempotency tùy semantics và failure model. Nếu side effect nằm ngoài cùng transaction boundary, chỉ đổi ordering chưa đủ để đạt exactly-once effect.

## 7. Wrong or misleading fixes

- Tăng retry count không giúp nếu message đã được acknowledged và không còn được redeliver.
- Catch exception rồi log nhưng vẫn complete message vẫn giữ nguyên data-loss window.
- Chỉ kéo dài lock duration xử lý symptom thời gian, không sửa ordering của durable work và acknowledgement.

## 8. Production implications

Với Azure Service Bus, RabbitMQ hoặc broker tương tự, cần hiểu rõ auto-complete/manual settlement, lock/visibility timeout, retry và dead-letter behavior. Consumer correctness phụ thuộc vào failure boundaries chứ không chỉ happy path.

## 9. Trade-offs

Acknowledge sau durable work thường tạo khả năng redelivery nếu process chết sau commit nhưng trước ack. Vì vậy production consumer thường cần idempotent handling hoặc deduplication cho side effect quan trọng.

## 10. What a Senior engineer should notice

Fix này chuyển failure mode từ **at-most-once-like loss** sang **at-least-once-like retry**. Đó thường là trade-off an toàn hơn, nhưng nó tạo yêu cầu mới: business operation phải chịu được duplicate delivery. Senior engineer cần reason về toàn bộ state machine thay vì chỉ di chuyển một dòng code.
