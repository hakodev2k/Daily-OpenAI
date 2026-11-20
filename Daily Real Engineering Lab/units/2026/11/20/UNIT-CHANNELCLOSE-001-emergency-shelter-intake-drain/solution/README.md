# Reference Solution — xem sau khi tự thử

## Symptoms
Hai nguồn accepted, chỉ một hồ sơ processed. Reader thoát, producer còn lại ghi lỗi.

## Evidence
Timeline gốc: north queued → writer complete requested → south write rejected → reader drained. Barrier khiến lỗi deterministic.

## Root cause
Coordinator hoàn tất channel writer trước khi producer thứ hai được phép gửi. Reader drain đúng các item đã enqueue, nhưng hồ sơ còn lại không thể enqueue.

## Why the fix works
Mở barrier, `await Task.WhenAll` mọi producer trước `TryComplete`. Reader drain cho tới completion. Khi producer throw, `TryComplete(exception)` báo lỗi downstream.

## How to verify
Sau khi tự sửa, chạy `./verify.ps1`. Có thể so với `solution/Program.cs` và tạm copy vào starter để kiểm chứng reference.

## Alternative fixes
Dùng coordinator sở hữu completion duy nhất; bounded channel cho backpressure; durable broker nếu cần bảo đảm qua process crash.

## Wrong fixes
`Task.Delay` che race; tăng channel capacity không sửa lifecycle; nuốt `ChannelClosedException` che mất dữ liệu; chỉ await reader không đợi producer.

## Production implications
Tách metrics accepted/enqueued/processed/failed. Định nghĩa shutdown, retry, cancellation, durability và idempotency trước khi hứa exactly-once.

## Trade-offs
Await producers tăng thời gian đóng batch nhưng bảo toàn dữ liệu; bounded channel giảm RAM nhưng tăng backpressure; durable queue tăng chi phí.

## Senior insight
Writer completion là trách nhiệm coordinator. Kiểm thử producer faults và shutdown ngoài happy path.
