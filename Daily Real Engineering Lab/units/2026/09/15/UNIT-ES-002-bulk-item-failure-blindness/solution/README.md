# Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix

## Symptoms
Worker báo batch thành công nhưng search index thiếu SKU.

## Evidence
Transport request hoàn tất, trong khi bulk response fixture có `errors: true` và hai item có status 429/400 cùng error payload.

## Root cause
Ứng dụng dùng thành công ở HTTP/transport layer làm đại diện cho thành công của toàn bộ bulk operation. Bulk API có response envelope thành công trong khi từng item có thể thất bại.

## Why the fix works
Reference implementation duyệt operation results, xác định item status thất bại và biến partial failure thành application-level failure có thể quan sát.

## How to verify
Áp dụng logic tương đương vào `starter/Program.cs`, sau đó chạy `./verify.ps1`. Fixture hiện tại phải tạo `FAILED_ITEMS=2`, chứa `SKU-101` và `SKU-103`, và process trả exit code khác 0.

## Alternative fixes
Với Elasticsearch client chính thức, dùng typed bulk response và kiểm tra item failures thay vì tự parse JSON. Production worker cũng có thể phân loại lỗi retryable và permanent để retry có chọn lọc.

## Wrong / Tempting Fixes
- Retry toàn bộ batch mù quáng: có thể lặp lại operation đã thành công và không giải quyết lỗi mapping permanent.
- Chỉ log `errors=true` nhưng vẫn mark job successful: monitoring vẫn sai contract.
- Chỉ dựa vào HTTP 2xx: đó chính là boundary gây mất failure signal.

## Production implications
Cần quyết định rõ semantics của batch: all-or-nothing ở application workflow, partial completion có checkpoint, hay per-item retry. Telemetry nên có failed item count, reason category và document identity an toàn để điều tra.

## Trade-offs
Fail cả job đơn giản nhưng có thể retry lại item đã thành công. Per-item retry chính xác hơn nhưng cần idempotency, retry classification và state/checkpoint phức tạp hơn.

## Senior insight
Một protocol envelope thành công không đồng nghĩa mọi operation bên trong thành công. Khi tích hợp batch API, phải xác định failure boundary ở đúng abstraction level và thiết kế retry/observability theo boundary đó.

## References
- Elasticsearch Bulk API: https://www.elastic.co/docs/api/doc/elasticsearch/operation/operation-bulk
