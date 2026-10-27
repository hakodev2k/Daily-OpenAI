# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce, ghi hypotheses và tự sửa `starter/DispatchCoordinator.cs`.

## 1. Symptoms

Lệnh A đã vào thiết bị, lệnh B có admission timeout bằng 0 nhưng vẫn chạy; `MaxConcurrentWork=2`. Khi A kết thúc, `SemaphoreFullException` xuất hiện ở đường hoàn tất lệnh A.

## 2. Evidence

Harness giữ A ở một gate do `TaskCompletionSource` kiểm soát, nên B luôn được thử khi A chưa rời critical section. `SecondWorkRan=True`, `SecondOutcome=Executed`, sau đó `FirstError=SemaphoreFullException`. Đây không phải race phụ thuộc tốc độ CPU.

## 3. Root cause

`SemaphoreSlim.WaitAsync(TimeSpan, CancellationToken)` trả về `Task<bool>`: `false` khi hết thời gian chờ mà **không** lấy được permit. Starter bỏ qua kết quả boolean, vẫn thực thi callback và gọi `Release()`. B làm tăng count dù chưa từng acquire, khiến A release sau đó vượt `maxCount=1`.

## 4. Why the fix works

Reference lưu `bool acquired`; nếu `false`, trả `Busy` trước khi thực thi callback và trước `try/finally` sở hữu permit. `finally` chỉ chạy sau acquire thành công. Nếu callback ném exception, permit vẫn được trả; cancellation trong lúc chờ không release permit không sở hữu.

## 5. How to verify

Từ root unit:

```powershell
./verify.ps1
dotnet run --project ./solution/Reference.csproj -- verify
```

Lệnh đầu kiểm tra **starter đã sửa**; lệnh thứ hai kiểm tra reference độc lập. Sau sửa, `FirstOutcome=Executed`, `SecondOutcome=Busy`, `SecondWorkRan=False`, `MaxConcurrentWork=1`, `FirstError=none` và các failure-path checks đều `True`.

## 6. Alternative fixes

- Với yêu cầu chờ vô hạn, `WaitAsync(CancellationToken)` không trả boolean; nhưng thay đổi này **không đáp ứng** hợp đồng `Busy` tức thì.
- Có thể dùng per-device bounded queue nếu sản phẩm muốn đợi và giữ thứ tự; cần quy định queue capacity, cancellation, timeout, shutdown và backpressure.
- Với nhiều instance, semaphore trong một process không đủ để serialize lệnh theo thiết bị; cần ownership/coordination ở hệ thống hoặc thiết bị, với fencing/idempotency thích hợp.

## 7. Wrong / tempting fixes

- Tăng `maxCount`: che exception nhưng cho phép nhiều lệnh cùng thao tác thiết bị.
- Thêm `Task.Delay` hoặc retry tùy ý: không sửa việc thực thi khi chưa acquire, có thể tạo thêm lệnh trùng.
- Bắt và bỏ qua `SemaphoreFullException`: giấu hậu quả, không phục hồi invariant.
- Chỉ lock quanh `Release()`: không tạo ownership của permit cho B.

## 8. Production implications

Đo riêng `admission_timeout`, `admission_busy`, `inflight_commands`, callback failure; không log token hay payload nhạy cảm. Cần xác định khóa điều phối theo `chargerId`, tránh dùng một semaphore toàn hệ thống nếu không chủ đích. Với nhiều replicas, cần hợp đồng idempotency phía thiết bị để tránh gửi trùng sau timeout mạng.

## 9. Trade-offs

`Busy` nhanh bảo vệ thiết bị nhưng buộc caller retry có kiểm soát. Chờ lâu giảm lỗi bận nhưng tăng latency và lượng request tồn đọng. Queue giúp điều tiết burst nhưng tăng state, độ trễ và chi phí vận hành. Chọn theo SLA và đặc tính lệnh (đọc trạng thái khác lệnh thay đổi firmware).

## 10. Senior engineer should notice

Phải phân biệt **thử acquire** với **đã sở hữu permit**. Invariant là callback chỉ chạy khi sở hữu permit, và mỗi acquire thành công có đúng một release. Cần test đường timeout, cancellation, callback exception, phục hồi sau lỗi và hành vi nhiều replicas — không chỉ happy path.

## References

- [SemaphoreSlim.WaitAsync](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim.waitasync)
- [SemaphoreSlim.Release](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim.release)
- [Semaphore and SemaphoreSlim](https://learn.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim)
