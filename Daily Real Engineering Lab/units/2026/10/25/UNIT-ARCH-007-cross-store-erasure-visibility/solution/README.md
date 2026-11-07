# Reference Solution — chỉ xem sau khi tự thiết kế

Đây là **một phương án có thể bảo vệ được**, không phải kiến trúc duy nhất đúng. Tài liệu sử dụng dữ liệu synthetic và yêu cầu product/legal xác nhận chính sách retention trước triển khai.

## 1. Symptoms và evidence

SQL commit lúc 11:03:06, search delete thành công lúc 11:03:10 nhưng export E-491 tạo trước yêu cầu gỡ được reindex lúc 11:05:10. Public search trả hồ sơ lúc 11:06:03. Redis cache lại response sai và link Blob đã phát hành vẫn có thể GET đến 11:13. Support đánh dấu COMPLETED quá sớm. Không có một lỗi SQL transaction đơn lẻ giải thích toàn bộ.

## 2. Root cause

Không tồn tại **một invariant public suppression được enforce trên mọi public read và mọi projection writer**. Search delete chỉ thay đổi một bản sao tại một thời điểm; reindex từ snapshot cũ có thể ghi đè. Redis và pre-signed URL có lifetime riêng. Hệ thống nhầm component-level ACK với end-to-end visibility guarantee.

## 3. Proposed architecture

- SQL duy trì request/decision record theo composite key (tenantId, recordId), version/epoch đơn điệu, trạng thái REQUESTED → SUPPRESSED → PURGING → PURGED hoặc HELD/REJECTED theo policy. Không xoá bằng cách mất dấu yêu cầu.
- Trong một SQL transaction: validate tenant/actor, ghi suppression intent và version, ghi outbox event. Retry cùng idempotency key trả cùng request/status; không làm tăng version vô cớ.
- **Public read gate:** search và detail chỉ trả record nếu một kiểm tra visibility đáng tin cậy xác nhận được phép hiển thị. Search filter phải chạy cả trên cache hits và trên kết quả từ index. Batch kiểm tra tối đa 20 IDs/request bằng keyed lookup theo (tenantId, recordId); đo p95. Nếu gate không truy cập được, fail closed cho protected records (có thể trả 503 hoặc lọc bỏ kết quả), không im lặng fail open.
- Search projection worker chỉ index record nếu source version còn hợp lệ và suppression state cho phép. Reject stale exports/events có version cũ; reindex cần reconcile với suppression authority trước khi publish hoặc swap alias. Không dùng thứ tự arrival của event làm version.
- Redis cache có thể invalidate theo tenant/record hoặc query; **không coi invalidate là hàng rào an toàn**. Cache hit vẫn qua visibility gate. Chọn TTL cho performance, không dùng TTL làm correctness guarantee.
- Attachment: không phát hành long-lived public SAS cho protected content. Đưa download qua authorized gateway kiểm tra suppression mỗi lần (hoặc cơ chế revocation thực sự có test). Đối với SAS đã phát hành, không hứa revoke tức thời nếu không có kỹ thuật khả thi: giữ request ở PENDING, chặn ở edge bằng cơ chế đã xác minh, hoặc chờ hết hiệu lực trước khi ACK. Phải kiểm chứng access từ bên ngoài, không chỉ kiểm tra database.
- Purge vật lý active stores chạy bất đồng bộ, idempotent, có retry/DLQ và reconciliation job. Legal hold giữ bytes theo policy nhưng không đồng nghĩa tiếp tục public display. Backup snapshots giữ 30 ngày theo policy; trước khi restore phục vụ traffic, replay suppression ledger/reconcile và chạy canary.
- Public suppression authority phải có độ bền và availability tương xứng với lời hứa; nếu mất primary region, restore suppression state trước khi bật public search ở DR region.

## 4. ACK contract và state machine

API DELETE ban đầu trả 202 + requestId + PENDING. Chỉ chuyển sang COMPLETED sau khi:
1. suppression decision đã durable;
2. mọi public read path có thể quan sát suppression (search, detail, cached hits, downloads);
3. các link đã phát hành đã được xử lý theo cơ chế revoke thực sự hoặc đã hết hiệu lực;
4. canary/verification không còn thấy record;
5. audit event không chứa raw PII và có correlation id.

Nếu không đáp ứng trong 2 phút, vẫn PENDING, cảnh báo và cung cấp trạng thái tiến độ; không nói đã xoá xong. Physical purge có thể tiếp tục sau ACK theo 24h policy nếu điều đó được giải thích rõ.

## 5. Sequence (reference)

```text
Client -> API: DELETE tenant/record + idempotency key
API -> SQL: transaction validate + suppression(version N) + outbox
API -> Client: 202 PENDING
Worker -> Search: delete/index guard using version N
Worker -> Cache: invalidate (best effort)
Read API -> Suppression authority: batch visibility check on every result
Download gateway -> Suppression authority: check before serving bytes
Verifier -> Public endpoints: synthetic canary / existing URL probes
Verifier -> SQL: COMPLETED only when visibility and link guarantees hold
Support -> Client: COMPLETED
Purger -> Blob/Search/SQL: idempotent cleanup (policy-aware)
Restore job -> Ledger: replay/reconcile before enabling traffic
```

## 6. Alternatives and trade-offs

| Option | Strength | Risk / when unsuitable |
|---|---|---|
| A. Synchronous delete in SQL + search + cache + Blob | Conceptually simple API flow | No atomic commit across stores; timeout and partial success; stale reindex can resurrect; SAS may remain valid |
| B. Durable suppression + read gate + async cleanup | Explicit safety invariant, recovery and bounded ACK semantics | Extra read latency, dependency availability, operational state machine; must benchmark 400 RPS x 20 IDs |
| C. Single SQL-backed search/read model | Fewer projection consistency boundaries | Full-text search features and 250ms p95 may not meet requirements; costly migration; Blob/link and backup remain separate |

Chọn B cho bài toán này nếu batch gate được benchmark và có HA/DR. Nếu hệ thống nhỏ hơn hoặc SQL full-text đáp ứng, C có thể tốt hơn. Không mặc định thêm Kafka, microservices hoặc Redis để giải quyết tính đúng.

## 7. Verification

- AC-01/05: chạy search với cache hit/miss trước và sau ACK, không xuất hiện R-8842.
- AC-02/04: replay export E-491 và out-of-order events, indexer từ chối stale version.
- AC-03/11: crash/retry cùng idempotency key; cuối cùng một suppression decision và eventual cleanup.
- AC-06: thử URL đã cấp từ một client bên ngoài sau ACK; không chấp nhận 200.
- AC-07: legal hold giữ dữ liệu nội bộ theo policy nhưng public visibility bị chặn.
- AC-08: restore snapshot cũ vào staging; không bật public endpoint trước khi replay/reconcile xong.
- AC-09: kill visibility dependency; xác nhận fail-closed behavior và alert.
- AC-10: benchmark 400 RPS, batch 20 IDs; đo p50/p95/p99 và query count.
- AC-12: cùng record ID ở tenant khác không bị ảnh hưởng.

Metrics: suppression_ack_latency_seconds, public_visibility_violation_count (canary), stale_projection_reject_count, purge_backlog_age_seconds, gate_unavailable_count, link_revocation_pending_seconds. Cảnh báo nếu visibility violation > 0 hoặc PENDING quá 2 phút.

## 8. Wrong / tempting fixes

- **Chỉ giảm Redis TTL:** giảm cửa sổ cache stale nhưng không chặn stale reindex hoặc SAS.
- **Chỉ gửi delete event thêm lần nữa:** có thể thắng tạm thời, nhưng writer cũ vẫn có thể ghi lại.
- **Trả 200 ngay khi SQL commit:** là trạng thái một component, không chứng minh public visibility.
- **Xoá vật lý ngay mọi backup:** thường không khả thi với immutable snapshots và retention; cần chính sách và restore reconciliation.
- **Dùng distributed transaction khắp các stores:** phức tạp, thường không được hỗ trợ; vẫn không giải quyết URL đã phát hành.
- **Chỉ tăng worker replicas:** tăng throughput, không đảm bảo ordering/correctness.

## 9. Production implications và rollback

Triển khai read gate ở shadow mode (chỉ đo, không thay đổi output) trên synthetic records, benchmark latency và log mismatch không PII. Sau đó canary tenant, enforce fail-closed, kiểm thử restore và link revocation, cuối cùng mở rộng rollout. Rollback không được vô hiệu hoá suppression đối với các record đã ACK; có thể rollback tính năng mới nhưng giữ safety gate hoặc tạm dừng public search. SRE sở hữu runbook/alerts; backend sở hữu schema, worker, API; privacy/legal duyệt retention và communication contract.

## 10. Senior / Architect observations

Sự khác biệt quan trọng là **public visibility**, **durable decision**, **physical deletion**, **legal retention** và **customer acknowledgment** là năm mốc khác nhau. Cần định nghĩa hợp đồng giữa các mốc, kiểm tra đường đọc thực tế và phân biệt safety (không lộ dữ liệu) với liveness (cuối cùng purge xong). Kiến trúc đơn giản nhất thỏa invariant và có thể vận hành bởi team nhỏ là lựa chọn tốt hơn một sơ đồ nhiều dịch vụ nhưng không có proof.
