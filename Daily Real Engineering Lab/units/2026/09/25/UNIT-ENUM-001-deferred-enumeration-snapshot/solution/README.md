# Reference Solution

## Symptoms
Audit count và processed set không nhất quán trong cùng logical batch.

## Evidence
Nguồn mutable thay đổi giữa hai lần consumption của query.

## Root cause
LINQ `IEnumerable<T>` query được deferred và đánh giá lại khi enumeration.

## Why fix works
Snapshot tại business boundary giữ membership ổn định.

## How verify
Chạy `verify.ps1`.

## Alternative fixes
Immutable input hoặc source API trả snapshot/version.

## Wrong or misleading fixes
Đếm lại chỉ che inconsistency; lock tùy tiện không định nghĩa business snapshot.

## Production implications
Deferred execution có thể đọc lại mutable source ngoài thời điểm dự kiến.

## Trade-offs
Snapshot tốn memory; streaming giảm memory nhưng semantics phải phù hợp.

## What a Senior engineer should notice
Xác định temporal/ownership boundary, không chỉ nhớ mẹo materialization.