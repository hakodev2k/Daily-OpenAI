# Acceptance / verification checklist

Bài làm đạt khi có bằng chứng cụ thể cho từng mục:

1. Ma trận tối thiểu 2 app versions × 3 schema phases, ghi rõ READ/WRITE và tình huống consumer trễ.
2. DDL additive trước; không rename/drop hoặc thêm NOT NULL không có default/backfill khi V1 vẫn cần cột cũ.
3. Chỉ rõ source of truth, mapping `amount_cents` sang `gross_cents`, cách cập nhật khi V1/V2 ghi đồng thời, và chống double-count.
4. Backfill theo batch, keyset/watermark, checkpoint và idempotency; 180M / 4k rows/s có lower bound 12.5 giờ (chưa tính overhead).
5. Phép đo reconciliation: source accepted count so với persisted unique count, tổng cents theo ngày/tuyến/currency, số rows NULL/mismatch, alert thresholds và cutover gate.
6. Rollback cho từng phase; rollback V2 không làm mất adjustment đã ghi; chỉ rõ khi nào rollback V1 không còn an toàn.
7. Consumer lag 72 giờ được phản ánh trong thời gian compatibility; chứng minh không còn writer/reader cũ trước contract.
8. Phân biệt PostgreSQL DDL locking/scan risk với giả định online; đưa ra thử nghiệm staging và statement_timeout/lock_timeout hợp lý.
9. Trình bày trade-off ít nhất hai phương án (dual columns/compatibility adapter, shadow table hoặc versioned event schema).
10. Ghi rủi ro, owner, trigger dừng rollout, và một quyết định go/no-go có số liệu.

**Self-test**: tự lấy 5 events ở evidence, chứng minh unique count và net totals theo từng bước; giả lập consumer V1 phát message sau 71 giờ và chỉ ra hệ thống vẫn ghi hợp lệ. Đây là verification của phương án do bạn viết, không phải kiểm thử một reference implementation.
