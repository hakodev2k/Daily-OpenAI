# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Khi cache đã có dữ liệu, read path ổn định. Khi một popular key bắt đầu ở trạng thái miss, nhiều caller đồng thời đều trả về dữ liệu đúng nhưng downstream loader bị gọi gần bằng số caller.

## 2. Evidence

Reproduction cho thấy `callers=12` và `loaderCalls` tăng mạnh trong cùng một burst. Vấn đề vì vậy không phải functional correctness của value mà là duplicated expensive work tại miss boundary.

## 3. Root cause

Cache-aside flow thực hiện `TryGetValue` rồi độc lập gọi loader cho mỗi caller bị miss. Không có coordination cho các concurrent miss của cùng key, nên trước khi caller đầu tiên populate cache, các caller khác đều bắt đầu cùng expensive operation.

## 4. Why the fix works

Reference solution dùng một in-flight `Task` cho mỗi key. Caller đầu tiên tạo load; các caller tiếp theo của cùng key await cùng task. Khi hoàn tất, value được cache và in-flight entry được loại bỏ. Coordination theo key nên key khác không bị serialize không cần thiết.

## 5. How to verify

Chạy `verify.ps1` trên learner-editable `starter/`. Với 12 concurrent callers cho cùng cold key, tất cả result phải đúng và `loaderCalls` phải bằng 1.

## 6. Alternative fixes

Có thể dùng per-key `SemaphoreSlim`, framework/library hỗ trợ request coalescing, hoặc distributed coordination nếu nhiều process/instance phải cùng chống stampede. Với production Redis, local single-flight chỉ bảo vệ một process; yêu cầu cross-instance cần đánh giá riêng.

## 7. Wrong or misleading fixes

- Tăng TTL chỉ làm symptom xuất hiện ít hơn, không loại bỏ burst khi miss xảy ra.
- Một global lock có thể giảm duplicate load nhưng serialize các key độc lập và tạo contention không cần thiết.
- Pre-warm mọi key có thể tốn tài nguyên và không giải quyết dynamic/hot keys nói chung.
- Scale downstream ngay lập tức có thể che symptom nhưng vẫn giữ amplification factor theo số caller.

## 8. Production implications

Cache stampede có thể biến một cache outage, expiry wave hoặc cold deployment thành load amplification lên database/API downstream. Cần cân nhắc TTL jitter, stale-while-revalidate, failure behavior, timeout budget và multi-instance coordination tùy hệ thống.

## 9. Trade-offs

Single-flight giảm duplicated work nhưng thêm coordination state và cleanup complexity. Per-process coordination đơn giản, nhanh nhưng không ngăn nhiều app instances cùng reload. Distributed locking/coalescing tăng operational complexity và phải thiết kế failure/lease semantics.

## 10. What a Senior engineer should notice

Cache hit ratio cao không đủ chứng minh cache design an toàn. Cần xem behavior tại transition state: expiry, cold start, dependency failure và concurrent miss. Đo amplification lên downstream trước khi chọn cơ chế phức tạp hơn.