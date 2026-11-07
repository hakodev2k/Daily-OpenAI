# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Worker nhận đủ ba jobs, log `started` xuất hiện, nhưng `ExecuteAsync` kết thúc ngay. Khi host shutdown, lifecycle task của `BackgroundService` đã hoàn tất nên host không còn task nào đại diện cho ba jobs đó. Process kết thúc trước khi chúng hoàn tất.

## 2. Evidence

Starter cho thấy thứ tự điển hình:

```text
worker: ExecuteAsync entered
job 101: started
job 102: started
job 103: started
worker: ExecuteAsync returning
host: shutdown requested
summary: started=3 completed=0
```

Điểm quan trọng không phải là có exception hay không, mà là execution task mà host đang quản lý đã kết thúc trước owned work.

## 3. Root cause

`ExecuteAsync` khởi tạo các `Task` bằng fire-and-forget (`_ = ProcessAsync(...)`) rồi trả `Task.CompletedTask`. Ba child tasks không còn nằm trong lifecycle task của `BackgroundService`.

Generic Host quản lý lifecycle của service thông qua task trả về từ `ExecuteAsync`; nó không tự động khám phá và chờ những task bị tách khỏi execution path.

## 4. Why the fix works

Reference solution giữ lại các task mà worker sở hữu và `await Task.WhenAll(ownedJobs)`. Vì vậy `ExecuteAsync` chỉ hoàn tất sau khi toàn bộ accepted work hoàn tất. Khi `StopAsync` chạy, host vẫn thấy service đang active và chờ execution task trong shutdown budget.

## 5. How to verify

Copy ý tưởng của bạn vào `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Expected invariant:

```text
summary: started=3 completed=3
```

`verify.ps1` kiểm tra chính learner-editable `starter/`, không kiểm tra reference solution.

## 6. Alternative fixes

- Nếu jobs đến liên tục, dùng bounded `Channel<T>`/queue và một consumer loop có ownership rõ ràng thay vì tạo task rời rạc.
- Nếu shutdown cần ngừng nhận job mới nhưng drain job đã nhận, tách `acceptance cancellation` khỏi `in-flight drain` và đặt shutdown budget rõ ràng.
- Nếu công việc phải sống qua process restart, chuyển ownership ra durable queue/message broker; host-local task tracking không cung cấp durability.

## 7. Wrong / Tempting Fixes

- **Tăng `HostOptions.ShutdownTimeout` nhưng vẫn fire-and-forget:** host vẫn không biết phải chờ task nào; tăng timeout không sửa ownership.
- **Thêm `Task.Delay` trước khi process exit:** chỉ che symptom bằng timing, không tạo lifecycle guarantee.
- **Bọc `ProcessAsync` trong `Task.Run`:** vẫn là detached task nếu không được await/tracked.
- **Nuốt cancellation hoặc exception:** làm observability tệ hơn và không giải quyết work ownership.

## 8. Production implications

Work đã được chấp nhận nhưng chỉ nằm trong memory có thể mất khi deployment, crash hoặc scale-in. Với work quan trọng về business, cần quyết định rõ đâu là boundary giữa in-process graceful completion và durable processing.

## 9. Trade-offs

Await toàn bộ owned tasks rất phù hợp cho bounded batch nhỏ như lab này. Với workload dài hoặc unbounded, cần queue, backpressure, concurrency limits và shutdown/drain policy; `Task.WhenAll` trên một tập vô hạn không phải kiến trúc phù hợp.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi “có await không”, mà phải xác định **ai sở hữu work**, **lifecycle nào đại diện cho completion**, **shutdown budget là bao nhiêu**, và **business có cho phép mất accepted work khi process chết hay không**. Đây là ranh giới giữa async syntax và production lifecycle semantics.
