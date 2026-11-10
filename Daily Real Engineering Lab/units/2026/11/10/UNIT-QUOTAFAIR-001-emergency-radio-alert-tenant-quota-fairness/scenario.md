# Scenario & evidence

Gateway gửi tối đa 120 bản tin/phút; không thể nâng công suất tuần này. RESCUE=P0, MAINT=P1, TEST=P2. API HTTP 202 chỉ xác nhận enqueue, không phải delivery. Retry có cùng tenantId + messageId. Backlog toàn hệ thống tối đa 240.

SLO: RESCUE p99 enqueue-to-delivery <=60 giây khi arrival <=60/phút, gateway online và burst trong giới hạn thỏa thuận. MAINT có tối thiểu 20 slots/phút khi backlog và RESCUE <=60/phút. TEST best effort. Offline 2 phút làm SLO không khả thi.

| Minute | Tenant | Received | Accepted | Sent | p99 seconds | Queue age seconds |
|---|---|---:|---:|---:|---:|---:|
| 10:00 | RESCUE | 30 | 30 | 18 | 88 | 92 |
| 10:00 | MAINT | 35 | 35 | 28 | 61 | 83 |
| 10:00 | TEST | 100 | 100 | 74 | 34 | 54 |
| 10:01 | RESCUE | 55 | 55 | 21 | 135 | 160 |
| 10:01 | MAINT | 30 | 30 | 27 | 80 | 100 |
| 10:01 | TEST | 95 | 95 | 72 | 51 | 98 |
| 10:02 | RESCUE | 45 | 45 | 24 | 177 | 190 |
| 10:02 | MAINT | 40 | 40 | 24 | 105 | 125 |
| 10:02 | TEST | 80 | 80 | 72 | 60 | 110 |

Đây là các mẫu quan sát, không đủ để chứng minh nguyên nhân. Hãy: (1) tính tổng sent và breach; (2) đưa 3 competing hypotheses và evidence còn thiếu; (3) thiết kế scheduler có guaranteed floors, work-conserving borrowing và queue bounds; (4) mô tả retry/deduplication, crash recovery và ambiguous device ack; (5) test burst, tenant isolation, retry, outage/recovery, rollback; (6) so sánh ít nhất hai phương án bằng ADR.

Không đổi giao thức thiết bị, không log payload nhạy cảm. Không hứa exactly-once delivery nếu thiết bị không hỗ trợ deduplication.

## Investigation workspace
Symptoms:
Evidence:
Hypotheses:
Missing metrics:
Proposed policy:
Verification matrix:
Trade-offs:
Rollback:
