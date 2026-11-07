# Reference Solution — chỉ xem sau khi đã reproduce và tự sửa

## Symptoms & Evidence
Timeline cho thấy nguồn phát W-17 version 1 → 2 → 3, nhưng delivery đến consumer là 1 → 3 → 2. `run.ps1` hiển thị trạng thái CLOSED version 3 bị thay bằng SUSPENDED version 2. CPU và cache không giải thích được vì lỗi xuất hiện ngay trong local in-memory projection.

## Root cause
Starter sử dụng **arrival-order last-write-wins** và chỉ nhớ EventId đã thấy. Message đến muộn vẫn ghi đè bản full snapshot mới hơn. HashSet EventId cũng không phát hiện cùng ID nhưng payload khác. Đánh dấu message đã xử lý trước khi kiểm tra tính hợp lệ khiến việc phân loại trạng thái trở nên khó tin cậy.

## Why the fix works
Mã trong [StatusProjection.cs](StatusProjection.cs) so sánh `Version` theo **SiteId**, kiểm tra trùng `EventId` kèm payload và chỉ áp dụng khi version tăng. Nó tách `IgnoredDuplicate`, `IgnoredStale` và `RejectedConflict`; chỉ tăng `AppliedCount` khi thực sự áp dụng. Đây là **in-memory reference**, không phải một production-grade durable implementation.

## How to verify
Sau khi thử phương án riêng, có thể so sánh bằng cách copy file tham khảo vào `starter/StatusProjection.cs`, rồi chạy `./scripts/verify.ps1`. Không chạy verify trên `solution/` riêng; harness phải tham chiếu starter. Kỳ vọng 8 contract PASS.

## Architecture decision / trade-offs
| Option | Lợi ích | Rủi ro |
|---|---|---|
| Arrival-order last-write-wins | Dễ viết, ít state | Sai khi redrive/reorder |
| Ordered partition theo SiteId | Giảm reorder trong đường bình thường | DLQ replay và reprocessing vẫn có thể vượt thứ tự; broker ordering không thay thế DB invariant |
| Monotonic per-site version + durable inbox | Chịu duplicate, reorder, crash; rõ invariant | Cần DB transaction, unique constraints, conflict policy và telemetry |

**Production design:** trong một DB transaction, insert `inbox(EventId, payloadHash)` có unique constraint, compare-and-set `projection(SiteId, Version, Status)` chỉ khi `incomingVersion > storedVersion`, ghi audit record và commit. Chỉ ack broker **sau commit**. Với duplicate, kiểm tra hash; nếu conflict, quarantine và báo động. Nếu crash sau commit trước ack, redelivery thấy inbox và không tạo side effect mới. Cần quyết định cách lưu stale/rejected event trong audit; đừng đánh dấu là đã áp dụng. Nếu dùng nhiều DB hoặc external side effect, transaction cục bộ không bao trùm tất cả: cần outbox cho publish sau commit và consumer idempotency.

**Full snapshot vs delta:** với full snapshot, version 3 có thể thay thế version 1 dù version 2 chưa đến. Với delta, bỏ qua version 2 có thể làm hỏng trạng thái: phải giữ gap, replay theo thứ tự hoặc rebuild từ nguồn chuẩn.

**Observability:** đếm stale, duplicate, conflict, projection lag theo nhóm nhỏ (không gắn EventId/SiteId làm metric labels). Log chi tiết với correlation ID có kiểm soát; alert conflict và bất thường redrive.

**Rollout:** shadow projection từ event log, so sánh theo SiteId/Version, chạy replay có kiểm soát, sau đó chuyển read traffic; rollback phải giữ invariant version và không dùng bản cũ hơn để ghi đè.

## Wrong / tempting fixes
- Chỉ tăng số lần retry: có thể làm reorder nhiều hơn, không sửa invariant.
- Sắp xếp batch in-memory: không bảo đảm thứ tự qua nhiều consumer, DLQ và restart.
- Dùng `lock` trong một process: không bảo vệ 4 instance hoặc crash.
- Bỏ qua toàn bộ redrive: mất audit và có thể mất cập nhật hợp lệ.
- Dùng EventId-only dedupe: không phát hiện stale snapshot có EventId khác.
- Chỉ dùng Redis TTL dedupe: TTL hết thì replay cũ có thể ghi đè lại.

## What a Senior/Architect should notice
Correctness nằm ở storage invariant, không ở giả định delivery-order. Chỉ khi business contract xác nhận event là **full snapshot** mới được áp dụng quy tắc version cao hơn thắng; delta events cần giải pháp khác. Mô phỏng kiểm tra semantics, chưa chứng minh durability, concurrent transaction isolation hay latency production.
