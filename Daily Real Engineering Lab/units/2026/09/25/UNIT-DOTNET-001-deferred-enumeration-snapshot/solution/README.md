# Reference Solution

## Symptoms
Audit count và processed set không nhất quán dù cùng một logical batch.

## Evidence
Nguồn mutable thay đổi giữa hai lần consumption của query.

## Root cause
LINQ query trên `IEnumerable<T>` được deferred; mỗi enumeration đánh giá lại pipeline trên trạng thái nguồn tại thời điểm đó.

## Why fix works
Materialize đúng tại business boundary tạo snapshot ổn định cho operation.

## How verify
Chạy `verify.ps1`; audit count và processed count phải bằng nhau.

## Alternative fixes
Dùng immutable input, chuyển ownership của batch, hoặc thiết kế source API trả snapshot/version rõ ràng.

## Wrong or misleading fixes
Gọi `Count()` thêm lần nữa chỉ che inconsistency. Lock tùy tiện có thể tăng contention mà không định nghĩa đúng business snapshot.

## Production implications
Deferred execution có thể khiến DB query hoặc mutable collection được đọc lại ngoài thời điểm mà code review tưởng rằng dữ liệu đã được chọn.

## Trade-offs
Snapshot dùng memory và có thể stale; deferred streaming giảm memory nhưng semantics phải phù hợp operation.

## What a Senior engineer should notice
Điểm quan trọng không phải `ToList()` như một mẹo, mà là xác định temporal/ownership boundary của dữ liệu.