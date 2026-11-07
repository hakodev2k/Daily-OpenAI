# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Batch task hoàn tất trong khi required item handlers vẫn đang chờ; exception của item không được batch caller quan sát.

## 2. Evidence
`batchTask.IsCompleted` trở thành `true` trước khi gate cho item work tiếp tục. Sau khi gate mở, item 2 fault nhưng batch đã trả success.

## 3. Root cause
`ProcessBatchAsync` tạo các task rồi bỏ ownership của chúng bằng discard. Completion contract của method vì vậy chỉ đại diện cho việc dispatch, không đại diện cho required work.

## 4. Why the fix works
Giữ các item task và await toàn bộ chúng làm cho batch task chỉ đạt terminal state khi required work đã đạt terminal state. Failure của item vì thế được truyền tới caller thay vì bị tách khỏi batch lifecycle.

## 5. How to verify
`verify.ps1` yêu cầu batch chưa complete trước khi gate mở, item failure được batch quan sát, và `batch-completed` không được công bố cho failed batch.

## 6. Alternative fixes
Nếu business requirement thực sự là enqueue-and-return, hãy chuyển ownership sang một durable queue/worker boundary và đổi completion semantics rõ ràng. Structured concurrency trong method chỉ phù hợp khi item work thuộc lifetime của batch operation.

## 7. Wrong or misleading fixes
Thêm `Task.Delay`, tăng timeout, hoặc chỉ bọc fire-and-forget body bằng `try/catch` không sửa completion contract. `Task.Run` cũng chỉ chuyển nơi chạy, không tự tạo ownership đúng.

## 8. Production implications
Detached work có thể bị mất khi process shutdown, exception khó correlate, telemetry báo success sớm và backpressure bị phá vỡ.

## 9. Trade-offs
Await toàn bộ work giữ semantics rõ nhưng batch latency bằng lifetime của required work. Durable dispatch giảm request coupling nhưng cần queue semantics, retry, idempotency và operational ownership.

## 10. What a Senior engineer should notice
Câu hỏi chính không phải “có chạy async không” mà là ai sở hữu task, completion của API hứa điều gì, failure đi đâu, và work có còn hợp lệ sau khi caller scope kết thúc hay không.