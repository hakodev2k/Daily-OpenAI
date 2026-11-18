# Reference Solution — chỉ xem sau khi tự làm

Đây là **một quyết định có thể bảo vệ**, không phải kiến trúc đúng duy nhất.

## Symptoms và evidence
p99 audit 14 giây vượt mục tiêu 5 giây ở p95 (khác percentile, không so trực tiếp). Autovacuum lag và IOPS spikes đáng điều tra, nhưng chưa đủ để khẳng định nguyên nhân. Có hai sự cố thiếu event khi business commit trước asynchronous publish: đây là rủi ro consistency độc lập với việc chia storage.

## Decision
Chọn **hybrid có điều kiện**: transactionally persist audit/outbox record trong cùng PostgreSQL transaction với thay đổi nghiệp vụ; worker chuyển bản sao có integrity metadata sang kho dài hạn; giữ hot partitions phục vụ truy vấn gần đây, archive giữ 7 năm với index caseId + timestamp; auditor gateway truy vấn hot/archive và ghi access logs. Chưa được triển khai production trước benchmark, legal review và DR drill.

## Root cause / design tension
Dùng operational DB làm cả high-churn OLTP lẫn long-retention audit tạo trade-off về vacuum, backup và truy vấn; tách audit không có transactional boundary lại gây thiếu event. Partitioning không tự giải quyết missing events hay legal erasure.

## Why it works / verification
Outbox cho phép worker retry sau commit mà không mất intent; archive ingest phải idempotent theo event ID, reconcile count/hash theo partition và case. Integrity dùng append-only manifest/chained hashes có key custody độc lập, kiểm tra tamper-evidence định kỳ; không tuyên bố chống mọi admin compromise. Run load test với 150M event scale, hot case query p95 < 250 ms, audit-by-case p95 < 5 s; kiểm tra tail latency và chi phí thực tế. Inject crash trước/sau archive acknowledgment và assert không thiếu/nhân đôi logical event. Restore drill kiểm tra RPO/RTO được chấp thuận.

## Alternatives
**A — PostgreSQL partitioning:** ít moving parts, tốt khi dataset/index/backup vừa sức và team nhỏ; cần thử partition pruning, index strategy, vacuum, backup.
**B — archive tách biệt:** giảm OLTP footprint nhưng index/rehydration, consistency, DR và quyền truy cập phức tạp; nếu lưu object thuần, truy xuất case ID có thể chậm.
**C — hybrid:** linh hoạt hơn nhưng vận hành hai đường đọc/ghi, reconcile và chi phí tăng; chỉ chọn khi benchmark chứng minh A không đủ hoặc retention buộc tách.

## Wrong / tempting fixes
Tăng instance DB ngay có thể mua thời gian nhưng không xử lý retention hay missing audit. Chỉ dùng TTL xóa sau 7 năm có thể vi phạm legal hold. Chỉ dual-write hai storage không có outbox/transaction protocol làm phát sinh gap. Đưa mọi audit vào object storage mà không có index có thể không đạt retrieval SLA. Các lựa chọn này không sai tuyệt đối, nhưng thiếu kiểm chứng theo constraints.

## Production implications / trade-offs
Mô hình 151,2M events × 1,2 KiB × 1,45 / 2^20 ≈ 251 GiB; cần bổ sung index, WAL, replicas, backup, compression thực tế, bloat, egress, nhân công và DR. Lập cost model trong trần 125 đơn vị/tháng trước quyết định. Privacy requests cần policy theo record/legal basis, selective redaction hoặc crypto-erasure khi được phép, và legal hold; không tự động xóa chứng cứ đang bị giữ.

## Change request
Region outage + truy xuất trong 30 giây đòi hỏi đo failover, replica/archive availability, index replication, key availability và query routing. Nếu không đạt cost ceiling, đề xuất tiered SLA, giảm scope hoặc thương lượng ngân sách; không hứa RTO 30 giây dựa trên lý thuyết.

## Senior engineer should notice
Phân biệt SLO percentiles; kiểm soát evidence consistency; audit access itself; restore test; unknown cost; quyền xóa vs nghĩa vụ lưu; không áp dụng giải pháp đắt hơn khi option A chưa bị loại bằng benchmark.