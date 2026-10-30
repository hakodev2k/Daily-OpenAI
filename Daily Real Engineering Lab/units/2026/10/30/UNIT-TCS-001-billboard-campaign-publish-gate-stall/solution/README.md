# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Baseline STALLED; cả publisher và subscriber chưa hoàn thành, delivered=0.

## 2. Evidence
reproduce.ps1 trả REPRODUCED; verify.ps1 trên baseline trả non-zero. Consumer được đăng ký trước producer, không có network I/O.

## 3. Root cause
TaskCompletionSource mặc định có thể chạy continuation inline trong TrySetResult. Publisher đang giữ SemaphoreSlim khi gọi TrySetResult. Continuation subscriber chạy trong stack publisher rồi gọi gate.Wait() đồng bộ. Subscriber đợi gate, trong khi publisher chỉ release gate sau khi TrySetResult trả về. Hai bên chờ vòng.

## 4. Why the fix works
Đổi trong starter/Program.cs:

    var published = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);

Continuation được schedule ngoài stack TrySetResult, cho phép publisher release gate trước khi subscriber chờ. Đây là scheduling contract, không phải đảm bảo một thread mới. Code tham khảo đầy đủ: solution/Program.cs.

## 5. How to verify
Chạy verify.ps1 trên starter đã sửa. Ba vòng phải COMPLETED, mỗi vòng delivered=1, hai Task completed. So sánh reference bằng lệnh:

    dotnet run --project .\solution\SignalGateLab.csproj -- --verify

## 6. Alternative fixes
- Hoàn tất signal ngoài critical section nếu thứ tự state và contract cho phép.
- Chuyển gate.Wait() sang await gate.WaitAsync() để không block continuation; review cancellation và invariant.
- Nếu cần nhiều subscribers, cân nhắc channel/queue có ownership rõ ràng.

## 7. Wrong fixes
- Tăng timeout: không phá vòng chờ.
- Task.Run bao quanh publisher: chuyển deadlock sang ThreadPool.
- ConfigureAwait(false): không bảo đảm continuation không chạy inline.
- Nâng semaphore capacity: có thể che bug và làm sai invariant.
- Swallow exception: không giải quyết vì không có fault.

## 8. Production implications
Không gọi callback không kiểm soát khi đang giữ critical section. Thêm deadline, tracing state transitions, stuck-job alert, idempotent delivery trước khi tự động retry.

## 9. Trade-offs
RunContinuationsAsynchronously có scheduling overhead nhỏ và thay đổi timing. Signal ngoài lock có thể đơn giản hơn nhưng phải giữ đúng state visibility và ordering.

## 10. Senior engineer should notice
Task completion scheduling và lock ownership là một phần contract. Test phải kiểm tra progress, correctness, và side-effect cardinality; không chỉ assert signal đã phát.

Tài liệu chính thức:
- https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1
- https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcreationoptions
- https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim
