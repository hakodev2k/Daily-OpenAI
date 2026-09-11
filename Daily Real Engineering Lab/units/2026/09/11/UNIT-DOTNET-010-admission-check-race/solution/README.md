# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Capacity được cấu hình là `1`, nhưng hai caller cùng có thể được báo `Accepted`. Semaphore vẫn tuần tự hóa execution, vì vậy symptom dễ bị hiểu nhầm là không có lỗi.

## 2. Evidence

Harness giữ hai caller tại cùng một điểm sau khi chúng đã quan sát capacity. Cả hai có thể thấy `CurrentCount > 0` trước khi bất kỳ caller nào acquire semaphore.

## 3. Root cause

Starter dùng một check-then-act sequence:

1. đọc `SemaphoreSlim.CurrentCount`
2. quyết định rằng request có thể tiếp tục
3. sau đó mới gọi `WaitAsync`

`CurrentCount` chỉ là snapshot. Observation và acquisition là hai operation tách rời, nên nhiều caller có thể cùng vượt qua preflight check. Caller đến sau không bị reject; nó âm thầm xếp hàng ở `WaitAsync`.

## 4. Why the fix works

Reference solution dùng `WaitAsync(0, cancellationToken)` để thử acquire ngay. Operation này vừa kiểm tra availability vừa chuyển ownership của slot trong cùng semaphore operation. Nếu không lấy được slot, caller nhận `false` và bị reject ngay.

## 5. How to verify

Copy logic tương đương reference solution vào `starter/AdmissionGate.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả cần có đúng một `Accepted` và một `Rejected`.

## 6. Alternative fixes

- `Wait(0)` có thể phù hợp trong synchronous code, nhưng không nên đưa blocking API vào async flow nếu không cần.
- `Channel<T>` bounded có thể phù hợp nếu business requirement là queue có giới hạn thay vì reject ngay.
- Một rate limiter/admission-control abstraction chuyên dụng có thể tốt hơn khi policy gồm permits, queue limit, timeout hoặc partitioning.

## 7. Wrong or misleading fixes

- Tăng semaphore capacity chỉ che symptom và thay đổi business policy.
- Thêm `lock` quanh việc đọc `CurrentCount` nhưng thả lock trước `WaitAsync` vẫn giữ check và acquisition tách rời.
- Giảm timing giữa hai request không sửa correctness; race vẫn tồn tại.
- Chấp nhận queueing rồi gọi đó là admission control làm thay đổi contract `reject immediately`.

## 8. Production implications

Check-then-act trên concurrent state có thể tạo overload amplification: hệ thống tưởng rằng đã giới hạn số request được nhận, nhưng thực tế lại tích lũy waiter bên trong. Điều này ảnh hưởng latency, cancellation behavior, shutdown và memory pressure.

## 9. Trade-offs

Immediate rejection tạo backpressure rõ ràng nhưng caller phải xử lý retry hoặc fallback. Queueing hấp thụ burst tốt hơn nhưng cần queue bound, timeout, cancellation và observability. Chọn semantics theo business contract, không chỉ theo API tiện dùng.

## 10. What a Senior engineer should notice

- "State observed" không đồng nghĩa với "state reserved".
- Concurrency policy phải được biểu diễn bằng atomic transition ở đúng boundary.
- Semaphore bảo vệ concurrency execution chưa chắc bảo vệ admission semantics.
- Test concurrency tốt nên ép interleaving quan trọng xảy ra deterministic thay vì dựa vào may rủi scheduler.
