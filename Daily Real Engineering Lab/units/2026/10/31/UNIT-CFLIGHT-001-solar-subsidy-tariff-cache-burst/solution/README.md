# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce và thử fix.** Đây là một phương án bảo vệ hợp đồng trong **một process**, không phải lời giải duy nhất cho mọi topology.

## 1. Symptoms

12 request đồng thời cùng vùng nhận biểu giá đúng, nhưng upstream được gọi nhiều lần cho cùng một đợt. Quota 4 req/s của đối tác có thể bị vượt.

## 2. Evidence

Starter: warm hit = True, burst upstream calls > 1, các quote vẫn nhất quán. Đo số lần gọi upstream, không chỉ đo correctness hay p95.

## 3. Root cause

Check TTL rồi gọi provider là hai thao tác riêng biệt. `ConcurrentDictionary` chỉ bảo đảm atomicity cho từng thao tác collection, không tự gộp các cache miss đồng thời. Khi entry hết hạn, mỗi request đều có thể bắt đầu fetch mới.

## 4. Why the fix works

`solution/TariffCache.cs` dùng per-key `Lazy<Task<TariffQuote>>` trong dictionary inflight. Mọi request cùng key chờ cùng một Task; factory của `GetOrAdd` không gọi provider ngay, nên các factory thua cuộc không gây side effect. Sau success/failure, xóa đúng cặp key+flight để tránh task lỗi bị cache vĩnh viễn và tránh xóa nhầm flight mới. Hai key khác nhau không dùng global semaphore.

## 5. How to verify

```powershell
./scripts/reproduce.ps1
./scripts/verify.ps1
dotnet run --project ./solution/Solution.csproj
```

Hai script đầu lần lượt kiểm tra starter gốc và starter do bạn sửa; lệnh cuối kiểm tra reference độc lập. Trên starter chưa sửa, reproduce pass và verify fail. Reference trả exit code 0 và `BurstProviderCalls=1`.

## 6. Alternative fixes

- Semaphore theo key có lifecycle và eviction đúng: khả thi nhưng dễ giữ lock/key vô hạn.
- Cache stale-while-revalidate: hợp lý nếu biểu giá cho phép stale, cần explicit staleness policy.
- Distributed lease + shared cache: cân nhắc khi nhiều replica cùng vượt quota toàn cụm; thêm failure mode khi lease holder chết.

## 7. Wrong / tempting fixes

- Global `SemaphoreSlim(1,1)`: có thể hạ call count nhưng làm hai vùng không liên quan phải chờ nhau, vi phạm hợp đồng.
- Tăng TTL vô hạn: che triệu chứng nhưng biểu giá lỗi thời.
- Dùng `ConcurrentDictionary.GetOrAdd(key, _ => provider.FetchAsync(key))` trực tiếp: factory có thể chạy nhiều lần dù chỉ một giá trị thắng.
- Chỉ retry 429: tăng tải khi upstream đang quá tải.

## 8. Production implications

Đo upstream fan-out, inflight join, error, 429 và p95. Quyết định per-key cancellation: một caller hủy không nên hủy công việc dùng chung của các caller khác nếu không có reference counting. Có thể cần bound inflight keys để chống key cardinality abuse.

## 9. Trade-offs

Giải pháp chỉ gộp request **trong một replica**. Với N replica, cùng key vẫn có thể tạo N call. Shared Redis cache, jitter, stale policy hoặc distributed coordination chỉ nên thêm khi telemetry chứng minh quota toàn cụm vẫn bị vượt.

## 10. What a Senior engineer should notice

Thread safety của collection không tương đương correctness của workflow. Thiết kế phải phân biệt request-level cancellation, shared work ownership, TTL freshness, failure eviction và topology nhiều replica. Chứng minh cả functional contract lẫn resource amplification trước rollout.
