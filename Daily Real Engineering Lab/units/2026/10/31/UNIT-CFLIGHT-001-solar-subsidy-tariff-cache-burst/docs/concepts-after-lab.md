# Concepts after lab

## Cache stampede và request coalescing

Một cache thread-safe vẫn có thể có nhiều cache miss đồng thời cho cùng key. In-process single-flight gộp công việc đang chạy, không phải distributed lock; nếu triển khai 8 replica thì vẫn có thể có tối đa 8 upstream fetch đồng thời cho một key.

## Trade-offs production

- **Global semaphore:** giảm upstream concurrency nhưng chặn key độc lập, làm tăng tail latency.
- **Per-key single-flight:** phù hợp cho một process; phải dọn inflight khi success/failure, tránh giữ exception Task vĩnh viễn.
- **Stale-while-revalidate:** giảm latency nhưng chỉ phù hợp nếu business chấp nhận dữ liệu cũ.
- **Distributed coordination:** chỉ cần khi quota áp dụng toàn cụm và dữ liệu đủ quan trọng; cân nhắc lock expiry, fencing, network partition, lease recovery, cost.
- **TTL jitter:** giảm đồng bộ hết hạn giữa nhiều key/replica, không thay thế cơ chế kiểm soát concurrency.

## Metrics

Đo cache_hit, inflight_join, upstream_calls_by_key_group, provider_429, p95, stale_age và error_rate. Không gắn userId hoặc key không giới hạn vào metric labels.

## Nguồn chính thức

- [Microsoft: ConcurrentDictionary.GetOrAdd](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd)
- [Microsoft: Lazy<T> thread safety](https://learn.microsoft.com/en-us/dotnet/framework/performance/lazy-initialization)
- [Microsoft: Task-based asynchronous pattern](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
