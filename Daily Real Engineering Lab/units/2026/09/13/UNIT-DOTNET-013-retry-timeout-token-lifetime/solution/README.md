# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Attempt đầu hết timeout đúng policy, nhưng retry tiếp theo bị dừng ngay dù simulated dependency rất nhanh.

## 2. Evidence

Starter log cho thấy token của attempt 2 đã ở trạng thái canceled ngay khi attempt bắt đầu. Thời gian retry kết thúc nhỏ hơn nhiều so với delay của dependency.

## 3. Root cause

Starter tạo một `CancellationTokenSource` cho toàn bộ retry loop rồi gọi `CancelAfter` lại trên cùng object. Sau attempt đầu timeout, source đó đã chuyển sang trạng thái canceled vĩnh viễn. Retry tiếp tục tái sử dụng token đã canceled.

## 4. Why the fix works

Mỗi attempt tạo một linked `CancellationTokenSource` mới từ token cấp operation. Timeout cục bộ vì vậy chỉ thuộc lifetime của attempt hiện tại, trong khi token bên ngoài vẫn có thể dừng toàn bộ operation.

## 5. How to verify

Copy cách sửa vào `starter/Program.cs`, sau đó chạy:

```powershell
./scripts/verify.ps1
```

Kỳ vọng attempt 1 timeout, attempt 2 thành công và process trả về exit code 0.

## 6. Alternative fixes

- Dùng helper riêng nhận outer token và tạo timeout scope cho từng attempt.
- Với production retry policy, đặt per-attempt timeout trong policy/pipeline miễn là lifetime và precedence giữa overall timeout và per-attempt timeout được xác định rõ.

## 7. Wrong / misleading fixes

- Tăng timeout thật lớn chỉ che triệu chứng và thay đổi policy latency.
- Bỏ token khỏi dependency làm mất khả năng dừng work.
- Catch `OperationCanceledException` rồi retry vô hạn không sửa token lifetime và có thể tạo retry storm.
- Chỉ tạo `CancellationTokenSource` mới mà không link outer token có thể khiến shutdown/request cancellation không còn propagate.

## 8. Production implications

Timeout, retry và cancellation là ba policy có lifetime khác nhau. Khi composable resilience được dùng trong worker/API, cần xác định rõ token nào đại diện cho operation, token nào chỉ đại diện cho attempt và exception/cancellation nào được phép retry.

## 9. Trade-offs

Per-attempt timeout giúp retry có budget độc lập nhưng tổng latency có thể tăng theo số attempt. Production code thường cần thêm overall deadline để giới hạn tổng thời gian của operation.

## 10. Senior engineer should notice

Senior engineer không chỉ sửa `CancellationTokenSource`; họ kiểm tra policy composition: overall deadline, per-attempt timeout, retry count/backoff, shutdown propagation, idempotency của operation và telemetry phân biệt timeout với caller cancellation.
