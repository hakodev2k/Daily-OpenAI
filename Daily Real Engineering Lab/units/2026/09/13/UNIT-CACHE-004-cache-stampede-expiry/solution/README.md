# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms

Functional output vẫn đúng, nhưng một burst concurrent request cho cùng hot key tạo ra nhiều downstream loads gần như đồng thời ngay sau cache expiry.

## 2. Evidence

Starter in ra `REQUESTS=24` và `DOWNSTREAM_CALLS` thường gần với 24 trong khi `RESULTS_CONSISTENT=True`.

Điểm quan trọng: correctness của response không chứng minh cache path khỏe về capacity.

## 3. Root cause

Cache-aside path chỉ kiểm tra `TryGet`. Khi key hết hạn, nhiều request cùng lúc đều quan sát cache miss trước khi request đầu tiên kịp reload và populate cache. Mỗi request sau đó tự gọi expensive downstream operation.

Đây là cache stampede/thundering-herd ở expiry boundary.

## 4. Why the fix works

Reference solution giữ một `Lazy<Task<Product>>` đang chạy cho từng key trong `_inflight`.

Request đầu tiên tạo reload operation. Các request đến đồng thời cho cùng key nhận cùng object và `await` cùng `Task`. Khi operation hoàn tất, result được cache và in-flight entry được loại bỏ.

Vì coordination là **per key**, request cho key khác không bị serialize bởi một global lock.

## 5. How to verify

Sửa `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Verification yêu cầu:

- `RESULTS_CONSISTENT=True`
- `DOWNSTREAM_CALLS <= 2`

Reference implementation kỳ vọng thường là `DOWNSTREAM_CALLS=1`.

## 6. Alternative fixes

Tùy constraints, các lựa chọn khác có thể hợp lý:

- cache library có built-in request coalescing
- stale-while-revalidate
- refresh-ahead cho hot keys
- distributed coordination nếu nhiều application instances cùng reload một shared expensive source
- TTL jitter để tránh nhiều key hết hạn cùng một thời điểm

Không có một lựa chọn tối ưu cho mọi hệ thống.

## 7. Wrong / tempting fixes

### Tăng TTL rất lớn

Có thể giảm tần suất incident nhưng không loại bỏ cơ chế load amplification khi key cuối cùng vẫn expire. Đồng thời tăng staleness risk.

### Dùng một global lock cho mọi key

Có thể chặn stampede nhưng biến các key độc lập thành một critical section chung và tạo head-of-line blocking.

### Scale out application ngay

Có thể làm vấn đề tệ hơn: nhiều instance có thể cùng miss và cùng reload dependency nếu coordination chỉ dựa trên cache miss thông thường.

### Retry downstream mạnh hơn

Retry không giải quyết duplicate concurrent reload; khi dependency đang chịu burst, retry còn có thể khuếch đại tải.

## 8. Production implications

Trong production cần theo dõi ít nhất:

- cache hit/miss rate
- downstream calls per logical key/request
- dependency latency/errors
- hot-key distribution
- expiry/refresh behavior

Nếu chạy multi-instance, process-local single-flight chỉ coalesce trong từng process. Cần đánh giá xem dependency có đủ capacity hay cần cross-instance strategy.

## 9. Trade-offs

Single-flight giảm duplicate work nhưng thêm coordination state và cần xử lý failure/cancellation semantics rõ ràng. Nếu shared in-flight operation nhận cancellation trực tiếp từ một caller, một request có thể vô tình hủy work đang được nhiều request khác dùng chung.

Production implementation thường cần tách lifecycle của shared reload khỏi cancellation của từng waiter.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi “cache có hoạt động không?” mà còn hỏi:

- behavior tại cache-miss boundary dưới concurrency là gì?
- một logical miss tạo ra bao nhiêu physical dependency calls?
- fix có giữ isolation giữa các key không?
- multi-instance topology thay đổi correctness/capacity story như thế nào?
- failure và cancellation của shared work được ownership bởi ai?
