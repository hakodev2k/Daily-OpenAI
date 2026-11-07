# Reference Solution — Deployment Slot & Database Migration Strategy

> Chỉ đọc sau khi đã tự viết decision proposal.

## 1. Symptoms / Risk Surface

Slot swap giải quyết application deployment boundary, không giải quyết database rollback boundary. Nếu migration thay đổi schema theo cách old production code không còn chạy được, một swap-back có thể đưa traffic về binary cũ nhưng database vẫn ở trạng thái mới.

## 2. Evidence

Các constraints quan trọng nhất là database dùng chung, migration kéo dài 3–8 phút, rollback application cần vài phút và không có DBA trực 24/7. Vì vậy strategy phải ưu tiên compatibility hơn là cố làm database rollback đồng bộ với slot swap.

## 3. Root Cause

Release hiện tại coi application artifact và database schema như một deployment unit có cùng rollback semantics. Thực tế chúng có lifecycle khác nhau: slot có thể swap gần như tức thời còn schema/data migration thường không thể đảo ngược an toàn với cùng tốc độ.

## 4. One Defensible Solution

Dùng **expand → deploy/migrate → contract** theo nhiều release phase.

### Phase A — Expand

Deploy migration chỉ thêm cấu trúc backward-compatible, ví dụ:

- thêm nullable column/table/index
- không rename/drop ngay field old code còn sử dụng
- tránh constraint mới làm old writes fail

Old code vẫn phải hoạt động sau phase này.

### Phase B — Deploy compatible application

Deploy new code vào staging slot, warm up và health-check. New code phải chạy được trên expanded schema. Nếu data cần chuyển đổi, thực hiện backfill theo batch/idempotent job thay vì buộc một migration transaction kéo dài chặn release path.

### Phase C — Swap + observe

Swap traffic sang new slot. Theo dõi error rate, latency, database errors, failed writes và business metrics. Trong observation window, old slot phải vẫn tương thích với database để swap-back còn an toàn.

### Phase D — Contract later

Chỉ ở release sau, khi đã xác nhận không còn binary/worker nào dùng schema cũ, mới drop/rename hoặc tighten constraints. Destructive change không nên nằm cùng release cần instant rollback.

## 5. Why This Works

Strategy tách schema evolution khỏi application cutover và giữ một compatibility window. Rollback chủ yếu là application rollback, không phải emergency reverse migration trên production data.

## 6. Verification

Trước production, kiểm tra tối thiểu:

- old version chạy được với expanded schema
- new version chạy được với expanded schema
- mixed-version read/write path không corrupt data
- migration/backfill idempotent khi retry
- staging warm-up không tạo side effect ngoài ý muốn
- swap-back sau Phase C vẫn pass smoke/integration tests

## 7. Alternatives

### Maintenance window

Có thể phù hợp với internal system có downtime budget rõ ràng. Không phù hợp với constraint release thường không được downtime.

### Blue/green cả database

Tạo database riêng cho mỗi environment cho rollback rõ hơn, nhưng data synchronization/cutover phức tạp đáng kể với write-heavy transactional workload. Chỉ đáng cân nhắc khi business requirement biện minh cho chi phí đó.

### Automatic down migration on rollback

Có thể dùng cho một số additive/reversible migration nhỏ, nhưng không nên là rollback mặc định với destructive/data-transforming migration vì có nguy cơ mất dữ liệu hoặc reverse transform không còn chính xác.

## 8. Wrong / Tempting Fixes

- Chỉ chạy migration trong staging slot rồi cho rằng production database chưa bị ảnh hưởng: nếu slots dùng chung database thì assumption này sai.
- Swap application trước rồi chạy breaking migration ngay: tạo race window giữa binary versions và schema.
- Luôn tạo `Down()` rồi xem đó là rollback plan: khả năng generate reverse SQL không chứng minh data rollback an toàn.
- Tăng timeout pipeline cho migration: chỉ xử lý duration, không giải quyết compatibility boundary.

## 9. Production Implications

Cần ownership rõ cho migration, feature rollout, backfill và contract cleanup. Telemetry nên phân biệt schema/SQL errors với application errors và theo dõi business invariant trong observation window.

## 10. What a Senior Engineer Should Notice

Senior engineer không chỉ hỏi “slot swap có zero downtime không” mà phải xác định **rollback unit thực sự là gì**. Khi application và database có rollback semantics khác nhau, release design phải tạo compatibility window thay vì giả định có thể đảo ngược toàn bộ hệ thống tức thời.

## Trade-offs

Expand/contract làm release kéo dài qua nhiều deployment và tạm thời tăng schema/code complexity. Đổi lại, nó giảm blast radius và làm rollback thực tế hơn cho team nhỏ không có DBA trực 24/7.
