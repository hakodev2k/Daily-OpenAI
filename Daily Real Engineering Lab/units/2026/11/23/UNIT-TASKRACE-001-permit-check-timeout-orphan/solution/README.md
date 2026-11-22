# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Deadline trả TIMEOUT nhưng `afterGrace=1`.

## Evidence
`Task.WhenAny` chỉ quyết định task nào hoàn thành trước; task còn lại không tự dừng. Delay mô phỏng downstream 180 ms; timeout 30 ms.

## Root cause
Tác vụ `ProcessAsync` được tạo với `CancellationToken.None`. Khi timeout thắng, code trả về mà không yêu cầu công việc dừng; side effect xảy ra muộn.

## Why the fix works
Dùng `CancellationTokenSource(deadline)`, truyền token xuyên suốt operation và await trực tiếp. `Task.Delay(180, token)` ném `OperationCanceledException` trước khi increment, do đó không ghi muộn.

## How to verify
Đọc `Program.cs` trong thư mục này, áp dụng thay đổi tương ứng vào `starter/Program.cs`, chạy `./scripts/verify.ps1`; cần PASS và `afterGrace=0`.

## Alternative fixes
Nếu thao tác có side effect không thể cancel sau khi commit, dùng idempotency key và state machine / transactional boundary; deadline ở tầng API không đủ đảm bảo rollback. Có thể thêm compensation khi phù hợp.

## Wrong / tempting fixes
Chỉ giảm timeout, tăng ThreadPool, hoặc bỏ qua kết quả của task không ngăn side effect. Fire-and-forget không phải cancellation. Chặn đồng bộ task có thể tăng latency và không xử lý được distributed commit.

## Production implications
Propagate `HttpContext.RequestAborted` cùng deadline có kiểm soát, log operation ID, kết quả cuối cùng, cancellation reason; đặt idempotency tại nơi side effect được commit.

## Trade-offs
Cancellation là cooperative, không đảm bảo rollback nếu DB đã commit. Cần xác định điểm không thể hủy và contract response khi trạng thái không chắc chắn.

## Senior engineer should notice
TIMEOUT là trạng thái quan sát của caller, không phải chứng cứ giao dịch đã abort. Thiết kế cần phân biệt timeout, cancelled, committed và unknown outcome.
