# Reference Solution — chỉ xem sau khi reproduce và tự thử

## 1. Symptoms
Khi thời gian xử lý vượt chu kỳ tick, nhiều batch cùng active. Dừng timer không đồng nghĩa tất cả công việc đã hoàn thành.

## 2. Evidence
Harness ghi `BATCH_START`, `BATCH_COMPLETE`, `maxActive`, `activeAfterStop`. Baseline thường có `maxActive >= 2`; không có retry, broker hoặc nhiều replicas.

## 3. Root cause
`System.Threading.Timer` gọi callback theo tick mà không chờ callback trước. Lambda `async` chuyển thành `async void` vì signature callback là `void`. Sau điểm `await`, timer không sở hữu `Task` của batch. `Dispose()` không biến callback thành awaitable shutdown.

## 4. Why the fix works
`solution/JobRunner.cs` sử dụng **một async loop**: await batch trước khi đợi tick tiếp theo bằng `PeriodicTimer`. Chỉ một luồng điều phối sở hữu batch; `StopAsync` cancel token rồi await loop. Điều này giữ `maxActive <= 1` trong một process. Cancellation cần được downstream tôn trọng.

## 5. How to verify
Chỉ sửa `starter/JobRunner.cs` theo hướng bạn chọn; chạy `./verify.ps1`. Nếu muốn self-check bản tham chiếu:
```powershell
dotnet run --project ./solution/TimerOverlapLab.csproj -- verify
```
`reproduce.ps1` vẫn phải PASS vì luôn chạy baseline bất biến.

## 6. Alternative fixes
- `BackgroundService.ExecuteAsync` với await và `PeriodicTimer` phù hợp ứng dụng ASP.NET Core.
- Dùng `SemaphoreSlim` single-flight quanh callback để **bỏ qua** tick trùng; phù hợp nếu chấp nhận bỏ lượt và quản lý shutdown rõ ràng.
- Queue bounded, một consumer nếu cần lưu backlog; tăng độ phức tạp và phải quy định overload policy.

## 7. Wrong / tempting fixes
- Chỉ tăng interval: giảm xác suất overlap nhưng không có invariant khi dependency chậm bất thường.
- Chỉ gọi `Timer.Dispose()`: ngăn tick mới nhưng không chờ callback đã chạy.
- Thêm `Task.Run` cho callback: không giải quyết ownership và có thể tăng cạnh tranh.
- Thêm `lock` quanh lời gọi trả về `Task`: lock kết thúc trước khi async work hoàn tất.

## 8. Production implications
Trong nhiều replicas, single-flight trong process không bảo đảm global single-flight; cần distributed lease hoặc idempotent operation theo business contract. Exception handling, retries, metrics và graceful shutdown timeout phải được thiết kế riêng.

## 9. Trade-offs
Sequential execution có thể giảm throughput và bỏ lỡ tick trong thời gian downstream chậm. Nếu business cần mỗi event được xử lý, một durable queue thích hợp hơn timer đơn giản. Cancellation là cooperative; downstream không tuân thủ có thể làm shutdown kéo dài.

## 10. Senior engineer checklist
Định nghĩa rõ trigger semantics (fixed rate vs fixed delay), overlap policy, backlog policy, at-least-once semantics, idempotency, cancellation ownership, graceful shutdown, metrics (active jobs / duration / skipped ticks) và multi-replica behavior.

## 11. Technical reference
- [PeriodicTimer API](https://learn.microsoft.com/en-us/dotnet/api/system.threading.periodictimer)
- [Timer API](https://learn.microsoft.com/en-us/dotnet/api/system.threading.timer)
- [Hosted services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)

**Validation:** static review only. This environment did not execute `dotnet` commands.
