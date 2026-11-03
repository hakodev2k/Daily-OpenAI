# Reference Solution — chỉ xem sau khi tự thiết kế

Đây là **một phương án khả thi**, không phải kiến trúc duy nhất.

## Symptoms và evidence
Lỗi INSERT tăng ngay sau DDL lúc 10:01 khi toàn bộ pods vẫn là V1, nên không thể quy cho CPU hoặc V2 rollout. V1 phụ thuộc `amount_cents`; rename phá hợp đồng SQL. Thêm `currency_code NOT NULL` trên bảng đang có dữ liệu mà không có default/backfill cũng không hợp lệ với dữ liệu hiện hữu (PostgreSQL sẽ từ chối nếu có rows). Dashboard không phát hiện missing writes vì chỉ cộng dữ liệu đã lưu.

## Root cause
Migration phá backward compatibility trước khi toàn bộ writers/readers được nâng cấp; release sequence giả định rằng chỉ app pods mới ghi, bỏ sót delayed consumer. Đồng thời không có reconciliation với upstream event ledger.

## Một kế hoạch expand → migrate → contract
**S0**: `amount_cents` hiện hữu. V1 chạy.

**S1 (expand)**: Giữ `amount_cents`; thêm nullable `gross_cents bigint`, `adjustment_cents bigint`, `currency_code text`. DDL additive có thể cần ACCESS EXCLUSIVE lock ngắn; đặt lock_timeout, quan sát lock wait trên staging. Với dữ liệu V1, đọc fallback `COALESCE(gross_cents,amount_cents)`; default adjustment = 0 và currency = VND chỉ tại tầng đọc khi chưa backfill. Không tự động thêm NOT NULL khi dữ liệu chưa hợp lệ.

Triển khai V2-compatible: writer V2 ghi `amount_cents` theo giá trị gross đồng thời ghi `gross_cents`, `adjustment_cents`, `currency_code` trong **cùng một transaction**. Writer V1 vẫn ghi `amount_cents`; V2 reader dùng fallback. Nếu có updates, cần trigger/adapter hoặc một cơ chế đồng bộ hai chiều có định nghĩa precedence, không chỉ dual-write INSERT. Tất cả event có stable id và unique constraint để duplicate delivery không tạo thêm rows. Nếu V1 đọc net, không thể hiển thị adjustment; giữ compatibility có giới hạn và không tuyên bố V1 có cùng semantics.

**Backfill**: Chạy batch theo `id` keyset (UUID không tuần tự theo thời gian vẫn dùng cursor order ổn định), ghi checkpoint cuối batch; dùng `UPDATE ... WHERE gross_cents IS NULL` để không ghi đè V2 rows. Batch commit riêng, giới hạn 4k rows/s, kiểm tra WAL/replica lag và dừng khi vượt budget. 180M / 4k ≈ 45.000 giây = 12.5 giờ *tối thiểu*. Nếu nhiều write đang diễn ra, cần scan/reconciliation lặp để bắt kịp delta.

**S2 (cutover)**: Sau khi backfill hoàn tất và count/sum reconciled, bật V2 reads cho toàn bộ services; ngừng V1 writes chỉ sau khi chứng minh không còn consumer cũ và đã drain/replay an toàn. Đợi ít nhất cửa sổ lag thực tế **72 giờ** cộng safety margin và kiểm tra telemetry, không chỉ 45 phút. Trước khi contract, có thể thêm CHECK constraint `NOT VALID`, validate sau, rồi NOT NULL theo kế hoạch kiểm tra lock/scan. Drop cột cũ chỉ trong release riêng có sign-off.

## Compatibility matrix minh họa
| Client | S0 | S1 expanded | S2 contracted |
|---|---|---|---|
| V1 writer | OK | OK | FAIL; chỉ cho phép khi V1 đã hết |
| V1 reader | OK | OK (old gross semantics) | FAIL |
| V2 compatible writer | Chưa triển khai | OK | OK |
| V2 compatible reader | Chưa triển khai | OK với fallback | OK sau contract |
| Delayed V1 consumer | OK | OK | FAIL; drain trước S2 |

## Verification
Đối soát source event IDs với persisted IDs (không chỉ row count nếu có duplicate); kiểm tra số reject, dead-letter, NULL/mismatch, tổng gross/adjustment/net theo ngày, tuyến và currency. Ví dụ năm events: A gross 1000 adj 0; B gross 2000 adj -200; C gross 500 adj +50; B duplicate; D V1 gross 700 adj mặc định 0. Kết quả unique = 4, net = 1000+1800+550+700 = **4050 cents**. So sánh cả trước/sau cutover; diễn tập V1 delayed writer tại giờ 71 trên S1 phải thành công.

## Rollback và giới hạn
Trong S1 trước V2 write: rollback app an toàn, giữ cột mới. Sau khi có adjustment V2: rollback app sang V1 có thể làm mất *khả năng đọc hiểu* adjustment, dù DB vẫn giữ nó; phải tắt feature adjustment hoặc duy trì adapter, không drop cột. Sau contract S2: rollback V1 không an toàn; dùng forward fix hoặc restore theo kế hoạch được kiểm chứng. Go/no-go dựa trên reconciliation 0 missing accepted events, 0 unexplained mismatches, consumer lag 0 cho version cũ, lock budget đạt.

## Alternative fixes và trade-offs
- **Compatibility columns + adapter**: rollout đơn giản, storage overhead/backfill lớn, semantics của old reader hạn chế.
- **Shadow table + CDC**: cutover linh hoạt, nhưng ordering, exactly-once illusion, reconciliation và ops phức tạp.
- **Versioned event schema**: giúp consumer migration, nhưng không thay thế kế hoạch DB compatibility.
- **Tempting wrong fix**: deploy V2 thật nhanh hoặc tăng pod count không giải quyết V1 delayed consumer; chỉ đổi dashboard query che mất missing records; rename rồi rollback DDL có nguy cơ mất thông tin mới.

## Production / Senior insight
Phải quản lý **hợp đồng giữa phiên bản** thay vì chỉ thứ tự migration. DDL lock, WAL, replica lag, nguồn sự thật và khả năng rollback là phần của release plan. Khi không thể biểu diễn dữ liệu mới bằng schema cũ, backward compatibility chỉ là tạm thời và phải được chặn bằng feature gate. Ghi rõ owner và thời điểm không thể quay lại.

## References
- PostgreSQL 15 ALTER TABLE: https://www.postgresql.org/docs/15/sql-altertable.html
- PostgreSQL 15 explicit locking: https://www.postgresql.org/docs/15/explicit-locking.html
- PostgreSQL 15 constraints: https://www.postgresql.org/docs/15/ddl-constraints.html
