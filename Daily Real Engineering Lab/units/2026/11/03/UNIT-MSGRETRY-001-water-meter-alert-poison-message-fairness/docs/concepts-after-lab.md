# Concepts after lab
**Head-of-line blocking:** một phần tử chưa xử lý ngăn phần tử sau tiến triển. Cần xác định boundary ordering (per-site hay global) trước khi đổi scheduling.

**Retry budget:** lỗi transient chỉ được thử lại có giới hạn, nên dùng backoff và jitter trong hệ phân tán thật; simulator dùng logical ticks để deterministic.

**Dead-letter không phải success:** nó là trạng thái terminal cần ownership, retention, audit và chính sách redrive. Redrive phải idempotent và tránh loop.

**Unknown failure:** không nên phân loại mọi exception là transient; để lỗi lập trình/invariant làm fail-fast và quan sát được.

**Production:** cần persistent queue, message visibility/lease, deduplication, transactional state transition, concurrency caps, alerting và SLO theo từng site; không thể suy ra exactly-once từ simulator.
