# Reference Solution — spoiler

## 1. Symptoms
API khởi động lại nhưng dữ liệu giao dịch, chứng từ và notification không cùng recovery point.

## 2. Evidence
Outage 10:07; DB WAL confirmed through 10:04 → **DB RPO 3 phút**. Object replicated through 09:55 → **object gap 12 phút**. Traffic reopened 10:58 → **RTO 51 phút**, vượt target 45 phút 6 phút. Health 10:42 không phải business-ready. Queue offset 510 không cung cấp timestamp nhất quán với DB.

## 3. Root cause
Các hệ lưu trữ phục hồi độc lập, không có atomic transaction xuyên PostgreSQL, object store và message queue.

## 4. Per-order analysis
| Order | Evidence | Safe action |
|---|---|---|
| P-101 | DB/object/event đầy đủ | Verify checksum, allow reads |
| P-102 | DISPENSED, PDF thiếu | Quarantine; tìm nguồn độc lập, xác thực chữ ký/hash |
| P-103 | DISPENSED, event thiếu | Rebuild notification từ outbox/delivery ledger, stable idempotency key |
| P-104 | Commit sau DB recovery point | Không tự tuyên bố recoverable; đối chiếu biên nhận ngoài và manual review |
| P-105 | PENDING | Giữ pending, không suy trạng thái từ event/object |

## 5. Why fix works — runbook
1. Incident commander đóng ingress writes, ghi incident ID và mục tiêu phục hồi.
2. **Fencing** primary cũ bằng quyền ghi/lease epoch cưỡng chế tại tất cả writer và network isolation; không chứng minh được single writer thì **NO-GO**.
3. DBA restore DB đến verified WAL; ghi checksum, checkpoint và transaction counts.
4. Storage owner restore object manifest/hash; queue owner xác nhận offset và dedup ledger.
5. Reconciliation read-only, quarantine bản ghi lệch; phân biệt recoverable, reconstructable và uncertain.
6. Remediation idempotent có approval, tuyệt đối không replay lệnh dispense; notification có business key và delivery ledger.
7. QA kiểm tra A1–A9 và audit trail; incident commander quyết định GO/NO-GO trước khi mở writes.
8. Theo dõi replication lag, backlog và duplicate side effects; có abort switch. Failback là migration riêng, không tự động.

## 6. Verification
Tính lại RPO/RTO và inventory; tabletop primary trở lại lúc 10:59 phải bị chặn ghi. RPO 2 phút mới không đạt vì DB gap 3 phút và object gap 12 phút.

## 7. Alternatives
- Tăng WAL archival và object replication, cảnh báo lag, restore drill: chi phí thấp hơn nhưng không bảo đảm cross-store atomicity.
- Transactional outbox, immutable object manifest và reconciliation: phục hồi có chứng cứ, tăng operational complexity.
- Synchronous multi-region quorum cho critical data: có thể giảm RPO, đổi lấy latency, cost và availability trade-offs; không giải quyết tự động object consistency.

## 8. Wrong fixes
Health green rồi mở ghi bỏ qua integrity/fencing. Chỉ tăng backup frequency không sửa lệch kho. Replay mọi event có thể tạo side effect trùng. Failback ngay khi primary trở lại có nguy cơ split-brain.

## 9. Production implications
SLO RPO/RTO theo data class, test restore định kỳ, checksum audit, owner rõ ràng, cảnh báo replication lag và manual-review queue.

## 10. Senior engineering judgment
RPO nghiệp vụ phụ thuộc dữ liệu và chứng từ, không chỉ DB. No-go là quyết định đúng khi không chứng minh được single writer hoặc integrity.
