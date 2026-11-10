# Reference Solution — spoiler

## Symptoms & evidence
Tổng sent mỗi phút đều bằng 120: 18+28+74, 21+27+72, 24+24+72. RESCUE arrival 30/55/45 <=60 nhưng p99 88/135/177 >60 giây. Queue age tăng 92→160→190. Accepted không chứng minh sent.

## Root cause hypothesis
Shared unbounded admission và thiếu isolation có thể để TEST cạnh tranh với RESCUE. CSV không chứng minh scheduler hiện hành; cần enqueue/dequeue/send timestamps, queue depth, retry attempts, gateway ack và burst profile. Giả thuyết khác: duplicated retries, batching/head-of-line blocking, clock skew.

## Decision & why it works
Chọn per-tenant durable bounded queues với scheduler work-conserving: reserve tới 60 slots/phút cho RESCUE khi có nhu cầu, tối thiểu 20 cho MAINT khi RESCUE <=60, phần còn lại TEST; unused slots được mượn nhưng không tích lũy entitlement. Enforce admission quotas và global queue bound 240; trả 429 + Retry-After khi vượt quota, 503 khi gateway outage và queue đầy. Không drop P0 âm thầm. Có starvation guard cho MAINT.

## Feasibility
Arrival <=60/phút không tự bảo đảm p99 <=60 nếu burst tức thời hoặc backlog lớn. Cần burst envelope và online assumption; outage 2 phút không thể đạt SLO, phải công bố degraded state và recovery policy.

## Idempotency
Unique durable key (tenantId,messageId) cùng payload hash; duplicate cùng hash trả lại trạng thái cũ, hash khác trả 409. Ghi dispatch attempt trước send, reconcile sau restart. Device ack lost có thể gây duplicate physical send; không hứa exactly-once nếu không có device-side dedupe.

## Verification
1. TEST burst 200/min, RESCUE 40/min: P0 không bị starvation.
2. RESCUE 50/min và MAINT backlog: MAINT >=20 sent/min.
3. Retry cùng messageId 10 lần: một logical record, không 10 scheduled messages.
4. Offline 2 phút: bounded queue, clear rejection/degraded telemetry, recovery ordered.
5. Shadow scheduling và canary; rollback khi RESCUE p99/queue age hoặc duplicate rate xấu đi.

## Alternatives and trade-offs
Shared FIFO + global rate limit đơn giản nhưng thiếu tenant isolation. Strict P0 priority giảm trễ nhưng có thể làm MAINT starvation. Reserved work-conserving scheduler phức tạp hơn, cần audit policy và capacity metrics.

## Wrong fixes
Thêm workers không tăng công suất 120/min; tăng HTTP timeout không tăng sent; trả 202 mọi request che giấu overload; thêm Redis không tự tạo fairness.

## Production implications
Metrics: queue age/depth, admitted/rejected/sent, retries, unknown ack, p99 theo tenant pseudonym; không log nội dung cảnh báo. Alert theo P0 queue age và device availability. Rollout shadow→canary→expand; rollback có kiểm soát.

## Senior engineer should notice
Phân biệt service fairness và admission fairness, chứng minh SLO bằng traffic envelope, và không suy ra nguyên nhân chỉ từ aggregate throughput.
