# Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix

## 1. Symptoms
Worker nhận job nhanh nhưng khi shutdown xảy ra, một phần side effect chưa hoàn tất.

## 2. Evidence
`Received` có thể đạt 20 trong khi `Processed` thấp hơn 20. Điều này tách “đã lấy khỏi queue” khỏi “đã xử lý xong”.

## 3. Root cause
Vòng worker khởi tạo `ProcessAsync` nhưng không giữ ownership của các `Task` đó. `RunAsync` có thể hoàn tất trong khi child operations vẫn chạy. Khi host/process kết thúc, không còn lifecycle contract bảo đảm chúng được drain.

## 4. Why the fix works
Baseline reference solution `await` từng operation. Worker chỉ chuyển sang job tiếp theo sau khi operation hiện tại hoàn tất, nên completion của worker phản ánh completion của work nó đã nhận.

Copy `solution/ProjectionWorker.cs` sang `starter/ProjectionWorker.cs` để đối chiếu.

## 5. How to verify
Chạy `./verify.ps1`. Kỳ vọng `Received: 20/20` và `Processed: 20/20`.

## 6. Alternative fixes
Nếu throughput yêu cầu concurrency, dùng bounded concurrency và theo dõi toàn bộ in-flight tasks; khi shutdown phải dừng nhận work mới theo contract phù hợp rồi drain các task đã nhận trong shutdown budget. Với durable broker, cần phối hợp settlement/ack với completion của side effect.

## 7. Wrong / tempting fixes
- Tăng shutdown timeout nhưng vẫn fire-and-forget: chỉ giảm xác suất, không tạo ownership contract.
- Thêm `Task.Delay` trước khi process thoát: timing workaround, không chứng minh completion.
- Dùng `Task.Run` cho mỗi job: đổi nơi chạy nhưng không giải quyết ownership.
- Nuốt exception của child task: làm incident khó quan sát hơn.

## 8. Production implications
Trong worker thật, queue thường là durable broker. Nếu message được ack trước side effect, crash có thể gây mất work; nếu ack sau side effect, redelivery có thể tạo duplicate. Vì vậy lifecycle ownership và idempotency thường phải được thiết kế cùng nhau.

## 9. Trade-offs
Sequential processing đơn giản và an toàn nhưng throughput thấp hơn. Bounded concurrency tăng throughput nhưng cần tracking, failure aggregation, cancellation policy và drain budget rõ ràng.

## 10. What a Senior engineer should notice
Senior engineer phân biệt “dequeued/received” với “durably completed”, xác định ownership boundary của asynchronous work, và thiết kế shutdown semantics thay vì dựa vào timing.
