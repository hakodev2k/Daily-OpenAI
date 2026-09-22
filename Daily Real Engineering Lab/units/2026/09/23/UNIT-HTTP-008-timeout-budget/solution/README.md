# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms
Khi dependency chậm, một request có thể chạy qua nhiều attempt nối tiếp và kéo dài đáng kể sau deadline mà caller mong đợi.

## Evidence
Starter tạo caller deadline 900ms nhưng `GetQuoteAsync` không dùng `requestCancellation`. Mỗi attempt lại có timeout 700ms độc lập; ba attempt có thể cộng dồn thành khoảng 2.1 giây.

## Root cause
Retry policy và per-attempt timeout không được ràng buộc bởi end-to-end request deadline. Cancellation ownership bị đứt tại client boundary.

## Reference fix
Tạo linked cancellation token giữa request token và attempt timeout; trước mỗi retry phải bảo đảm request token chưa bị cancel. Trong hệ thống thực tế nên tính remaining budget từ một absolute deadline và chỉ retry khi còn đủ thời gian có ý nghĩa.

Ví dụ phần lõi:
```csharp
requestCancellation.ThrowIfCancellationRequested();
using var attemptTimeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(350));
using var linked = CancellationTokenSource.CreateLinkedTokenSource(requestCancellation, attemptTimeout.Token);
return await dependency.GetQuoteAsync(mode, attempt, linked.Token);
```
Khi catch `OperationCanceledException`, nếu `requestCancellation.IsCancellationRequested` thì rethrow ngay; chỉ retry khi cancellation đến từ attempt timeout và operation còn budget.

## Why the fix works
Caller deadline trở thành giới hạn chung cho toàn operation. Khi request hết useful lifetime, cancellation được truyền xuống dependency và retry loop không tạo thêm work.

## How to verify
Chạy `./verify.ps1`. Failure path phải kết thúc xấp xỉ caller budget thay vì cộng dồn ba timeout. Healthy path và fast-recovery path vẫn phải thành công.

## Alternative fixes
- Dùng absolute deadline (`DateTimeOffset`/monotonic elapsed budget) và tính timeout còn lại cho mỗi attempt.
- Đặt resilience pipeline ở một boundary duy nhất với total timeout + retry policy, thay vì nhiều timeout độc lập ở nhiều layer.
- Với dependency có latency cao ổn định, có thể không retry synchronous request mà chuyển sang degraded response hoặc asynchronous workflow tùy business contract.

## Wrong / Tempting Fixes
- Tăng caller timeout lên 3 giây: chỉ che mismatch và tăng resource occupancy.
- Tăng số retry: làm tail latency và load amplification tệ hơn.
- `Task.Run` quanh HTTP call: không tạo timeout budget và không sửa cancellation propagation.
- Scale out ngay: tăng capacity nhưng không loại bỏ wasted work.

## Production implications
Timeout, retry và cancellation là một capacity-control contract. Retry làm tăng số request xuống dependency; khi outage xảy ra, budget sai có thể biến latency incident thành load amplification incident.

## Trade-offs
Budget quá ngắn làm giảm success rate khi dependency chỉ chậm tạm thời. Budget quá dài giữ resource lâu và làm tail latency xấu. Retry chỉ đáng giá khi failure có khả năng transient, operation an toàn để retry, và còn đủ time budget.

## What a Senior engineer should notice
Không review timeout từng call một cách cô lập. Hãy review end-to-end deadline, retry multiplication, cancellation propagation, idempotency và downstream capacity cùng nhau.