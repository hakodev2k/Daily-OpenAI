# Reference Solution — chỉ xem sau khi tự reproduce và thử fix

## 1. Symptoms
Một acknowledgement hợp lệ mất khoảng thời gian tương đương thao tác audit chậm, dù producer chỉ định hoàn thành yêu cầu đang chờ. Khi tăng tải, các acknowledgement khác có thể bị chặn tại cùng một gate.

## 2. Evidence
Trong starter, `auditInsideGate=True` dù audit nằm trong method đang `await`. `producerElapsedMs` thường bao gồm độ trễ mô phỏng ~60 ms. Đây là evidence về **execution context của continuation**, không phải lỗi TCP hoặc database.

## 3. Root cause
`TaskCompletionSource<T>` mặc định **có thể chạy continuation đồng bộ** trên thread gọi `TrySetResult`. Producer gọi `TrySetResult` khi đang giữ `Monitor` lock. Continuation sau `await` thực hiện audit đồng bộ trên cùng thread trước khi producer rời critical section. `ConfigureAwait(false)` không bắt buộc continuation phải chạy asynchronous; nó chỉ điều chỉnh việc capture context.

## 4. Why the fix works
Thay khởi tạo `TaskCompletionSource<PalletReceipt>` bằng:

```csharp
new TaskCompletionSource<PalletReceipt>(
    TaskCreationOptions.RunContinuationsAsynchronously)
```

Continuation được schedule thay vì thực thi inline bên trong `TrySetResult`. Producer có thể hoàn tất phần critical section mà không thực thi công việc audit của consumer. Xem [mã reference](ReceiptCoordinator.cs). Chỉ copy phần sửa vào `starter/ReceiptCoordinator.cs`, không thay `verify.ps1` hoặc test harness.

## 5. Verification
Chạy `./verify.ps1`. 16 case phải có `accepted=True`, `auditCount=1`, `auditInsideGate=False` và exit 0. Thời gian producer thường giảm nhưng không dùng timing làm assertion tuyệt đối. Thử thêm nhiều pallet đồng thời để đánh giá throughput và contention.

## 6. Alternative fixes
Có thể lấy completion source ra khỏi dictionary dưới lock, rồi gọi `TrySetResult` **sau khi đã thả lock**. Đây là một phương án hợp lệ, nhưng phải bảo toàn atomicity của việc claim receipt và xử lý duplicate/timeout. Dùng `RunContinuationsAsynchronously` tại boundary thường giúp giữ contract rõ ràng và hạn chế callback inline từ các call-site khác.

## 7. Wrong / tempting fixes
- Chỉ thêm `ConfigureAwait(false)`: không bảo đảm continuation không inline.
- Tăng số ThreadPool threads: không sửa việc producer thực thi công việc của consumer trong critical section.
- Bỏ lock không phân tích: có thể làm hỏng matching và duplicate suppression.
- Giảm thời gian audit: có thể che triệu chứng nhưng coupling giữa producer và consumer vẫn tồn tại.
- Đưa toàn bộ `Accept` vào `Task.Run`: chuyển contention sang worker, tăng overhead và không sửa contract.

## 8. Production implications
Dưới tải cao, critical section dài làm tăng queueing latency; nếu callback chặn I/O, tác động có thể lan rộng. Bổ sung metrics về thời gian giữ lock, thời gian chờ, audit latency và tỉ lệ duplicate; dùng cancellation/timeout rõ ràng cho pending receipts.

## 9. Trade-offs
Asynchronous continuations có scheduling overhead và làm thay đổi timing/ordering quan sát được. Không được giả định callback hoàn tất trước khi `Accept` trả về. Contract nghiệp vụ nên được xác định bằng task completion và acknowledgement, không bằng side effect đồng bộ ngẫu nhiên.

## 10. Senior engineering observation
Phân biệt **data synchronization** (lock bảo vệ dictionary) với **completion notification** (đánh thức consumer). Kiểm tra những điểm gọi callback/event/Task completion khi đang giữ lock, không chỉ tìm `lock` quá lớn. Một test xác nhận kết quả cuối cùng là chưa đủ: phải kiểm tra cả execution boundary.

## Tham khảo
- Microsoft Learn — TaskCompletionSource<T>: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1
- Microsoft Learn — TaskCreationOptions: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcreationoptions
- Microsoft Learn — ConfigureAwait FAQ: https://devblogs.microsoft.com/dotnet/configureawait-faq/
