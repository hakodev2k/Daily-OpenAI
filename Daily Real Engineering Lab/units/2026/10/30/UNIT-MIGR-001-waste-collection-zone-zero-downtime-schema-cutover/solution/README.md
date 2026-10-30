# Reference Solution — SPOILER

**Chỉ xem sau khi tự phân tích và verify.** Đây là một thiết kế có thể bảo vệ, không phải kiến trúc duy nhất đúng.

## Symptoms và evidence
T0: writer cũ không thỏa schema mới. T1: dữ liệu cũ chưa sẵn sàng. T2: backfill và write đồng thời có thể lost update. T3: mapping không đơn trị. T4: rollback app không đồng nghĩa rollback dữ liệu. CPU/p95 trong evidence cho thấy backfill phải kiểm soát tải.

## Root cause
Thay schema và contract ứng dụng trong một bước khi writer cũ còn hoạt động; backfill dựa trên snapshot thiếu kiểm soát version; mapping mơ hồ bị coi là đơn trị; đường rollback không giữ compatibility.

## Phương án tham chiếu: expand → bridge → backfill → reconcile → cutover → contract

1. **Expand:** thêm `zone_id uuid NULL`, trạng thái review, bảng mapping đã duyệt và versioned. Giữ `service_zone`. Chưa NOT NULL hoặc drop. DDL cần đánh giá lock; `CREATE INDEX CONCURRENTLY` chạy ngoài transaction block.
2. **Bridge:** do V1 ghi trực tiếp DB, dual-write trong V2 *một mình* chưa đủ. Có thể dùng trigger DB tạm thời được test kỹ để đồng bộ text→ID khi mapping duy nhất, đánh dấu unresolved khi không duy nhất; V2 vẫn ghi text tương thích để rollback. Ngăn trigger recursion, tránh vòng cập nhật và đo overhead. Phương án khác: chuyển tất cả writer qua API adapter trước migration.
3. **Backfill:** batch nhỏ có checkpoint, pause/resume và rate limit. Đọc row hiện hành với lock trong transaction, hoặc compare-and-swap theo revision đáng tin cậy và retry conflict. Không dùng snapshot cũ ghi đè update mới. `WHERE zone_id IS NULL` một mình không đủ cho mọi race. Sau pass đầu, chạy catch-up/reconciliation và test idempotency khi crash.
4. **Reconcile:** kiểm tra mapping text/zone_id trên hàng verified, đếm mismatch và unresolved, shadow-read giữa V1/V2. 110.000 unmatched và 50.000 ambiguous phải được review; không chọn ngẫu nhiên. Route chưa verified không tự dispatch.
5. **Cutover:** V2 đọc ID với hàng verified; giữ đường xử lý an toàn cho unresolved. Theo dõi số V1 writers, mismatch, quarantine backlog, p95, CPU, wrong dispatch. On-call có quyền abort.
6. **Contract:** chỉ loại bỏ text và bridge sau khi không còn V1 writer/rollback dependency, có data gate, review và recovery drill. Nếu unresolved còn tồn tại, cần contract được phê duyệt rõ ràng trước khi áp NOT NULL. Không drop chỉ vì deploy xanh.

## SQL sketch — minh họa, chưa chạy
```sql
ALTER TABLE pickup_route ADD COLUMN zone_id uuid NULL;
-- Tạo bảng mapping đã duyệt và cơ chế compatibility riêng.
BEGIN;
SELECT service_zone, updated_at
FROM pickup_route WHERE id = 1205 FOR UPDATE;
-- Chỉ ánh xạ nếu có đúng một mapping hợp lệ trên giá trị hiện hành.
-- Ghi trạng thái review thay vì đoán zone khi ambiguous/unmapped.
COMMIT;
```
Đây không phải trigger production-ready. Cần integration tests về isolation, lock duration, deadlock, mapping index và tải ghi trước triển khai.

## Verify A–F
A: V1 ghi được, bridge bảo toàn read. B/C: quarantine, không dispatch sai. D: row lock hoặc revision CAS + catch-up chống stale overwrite. E: checkpoint/replay và throttle theo SLO. F: V2 giữ text tương thích; contract gate chặn drop.

## Alternatives and trade-offs
| Option | Benefit | Cost / risk |
|---|---|---|
| Rename + NOT NULL ngay | Ít bước | V1 hỏng, rollback khó |
| Expand-contract + DB bridge | Tương thích direct V1 writes | Trigger phức tạp, write overhead |
| Expand-contract + API adapter | Logic dễ test tập trung | Phải di chuyển mọi V1 writer trước |
| CDC/streaming pipeline | Tách write path | Lag, vận hành phức tạp, vượt ngân sách |

## Wrong / tempting fixes
- **Chỉ NOT NULL:** chặn V1 và hàng cũ.
- **Backfill một lượt:** bỏ qua concurrent updates.
- **Chọn mapping đầu tiên:** dispatch sai.
- **Chỉ WHERE zone_id IS NULL:** không bảo vệ snapshot stale khi text đã đổi.
- **Rollback toàn DB:** có thể làm mất write hợp lệ.
- **Drop khi CI xanh:** không chứng minh hết writer cũ.

## Production implications
DBA được pause backfill khi CPU > 80% hoặc p95 > 250 ms kéo dài 5 phút (ngưỡng giả định cần load test); on-call dừng cutover khi verified mismatch > 0 hoặc có wrong dispatch. Tech lead và business operations phê duyệt irreversible contract. Audit mapping version, quarantine backlog và rollback drill.

## Requirement change
Vùng hợp nhất có hiệu lực theo thời gian: giữ identity/assignment history, dùng `effective_from`/`effective_to` hoặc snapshot zone bất biến tại dispatch event. Dùng event time đã xác minh, không dùng thời gian cập nhật record. Ngày 2026-11-30 giữ zone cũ, 2026-12-01 dùng Z-09. Không sửa ngược lịch sử.

## Senior engineer observations
Schema là contract xuyên phiên bản và thời gian. Data gate quan trọng hơn deployment status. Cần đo correctness, availability, rollback và safety độc lập. Tránh streaming platform mới khi đội ngũ và tải chưa cần.

## Official references
- https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
- https://www.postgresql.org/docs/15/sql-altertable.html
- https://www.postgresql.org/docs/15/sql-createindex.html

**Validation:** static review; không chạy PostgreSQL hay benchmark.
