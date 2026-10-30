# Reference Solution — chỉ đọc sau khi tự reproduce và thử fix

## 1. Symptoms

Job báo hoàn thành khi gateway vẫn đang chờ và không truyền được lỗi `PARTNER_REJECTED` cho caller. Happy path có thể chạy đúng nên smoke test đơn thuần bỏ sót lỗi.

## 2. Evidence

`harness/` giữ gateway ở một `TaskCompletionSource` chưa được release. `CASE=WAITS_FOR_SEND` cho thấy job hoàn tất trước side effect; `CASE=PROPAGATES_FAILURE` chứng minh lỗi downstream không đi qua job boundary. Đừng dùng thời gian chạy tuyệt đối làm bằng chứng duy nhất.

## 3. Root cause

`Task.Factory.StartNew(async () => ...)` chọn delegate trả `Task`, dẫn đến kết quả `Task<Task>`. `await` trên outer task chỉ chờ delegate tạo inner task, không chờ hoạt động gửi bên trong. Lỗi xảy ra ở inner task không được job sở hữu/await.

## 4. Why the fix works

Tham khảo [ReportDispatcher.cs](ReportDispatcher.cs): trả trực tiếp `Task` từ `IReportGateway.SendAsync` để caller sở hữu đúng vòng đời. Khi caller await, nó chờ side effect thật và nhận exception. Không cần lập lịch sang ThreadPool cho I/O async đã có `Task`.

## 5. How to verify

Chỉ sửa `starter/ReportDispatcher.cs`; chạy `./verify.ps1`. Cả ba case phải PASS. `reproduce.ps1` được dùng để chứng minh baseline trước khi sửa, không dùng thay verify.

## 6. Alternative fixes

Nếu thật sự cần scheduling riêng, có thể `await Task.Factory.StartNew(async () => ..., ...).Unwrap()`; nhưng thường thêm overhead và phức tạp không cần thiết. `await _gateway.SendAsync(token)` trực tiếp cũng đúng, cho phép bổ sung logging/metrics quanh await.

## 7. Wrong / tempting fixes

- `Task.Run` mà không await: vẫn bỏ rơi side effect.
- `Task.Delay` sau khi schedule: chỉ che race và phụ thuộc timing.
- Nuốt exception trong delegate rồi báo success: làm sai hợp đồng vận hành.
- Retry vô hạn: có thể nhân đôi side effect, không sửa ownership.
- Chỉ sửa harness để PASS: không sửa đường production.

## 8. Production implications

Scheduler chỉ acknowledge sau khi task thực sự hoàn tất. Cần phân biệt lỗi transient/permanent, idempotency key cho retry và cancellation đi tới gateway. Không kết luận gửi thành công chỉ vì đã enqueue công việc.

## 9. Trade-offs

Trả trực tiếp `Task` đơn giản, ít allocation và đúng ownership nhưng không có điểm bọc để thêm telemetry. `async/await` trực tiếp rõ ràng khi cần logging, `try/finally`, hoặc phân loại lỗi. `StartNew(...).Unwrap()` hợp lệ trong trường hợp scheduling thật sự cần, nhưng dễ tăng độ phức tạp.

## 10. Senior engineer should notice

`Task<Task>` là tín hiệu review quan trọng. Thiết kế API phải thể hiện completion boundary, exception ownership, cancellation và retry semantics. Đặt failure-path tests với gateway bị chặn và gateway trả lỗi trước khi tối ưu throughput.

## References

- [Task.Factory.StartNew](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskfactory.startnew)
- [TaskExtensions.Unwrap](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskextensions.unwrap)
- [Async return types](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types)
