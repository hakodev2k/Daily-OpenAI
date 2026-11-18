# Quality / verification rubric
1. Đánh dấu GET 10:03 của standard và 10:01:01 của sensitive là vi phạm; 10:01:20 standard còn trong grace period.
2. Ba giả thuyết phân biệt: stale DB, cache/CDN, bearer token; có phép thử bác bỏ từng giả thuyết.
3. Threat model: leaked URL, replay, shared link, Range, CDN, compromised issuer.
4. Hai phương án có sequence và decision matrix đo latency, audit, revocation, cost, availability.
5. Sensitive GET mới luôn đi qua policy enforcement; không redirect SAS cho client.
6. Standard SLA được chứng minh bằng TTL + skew + cache behavior hoặc thay kiến trúc.
7. Có P99 revoke-to-deny, load test 1,000/min, 80 MB streaming, failure-path.
8. Có rollout canary và rollback fail-closed.
9. Hai change request dẫn đến quyết định nhất quán.
**Pass:** 8/9, bắt buộc 1, 4, 5, 8. Không ghi nhận mastery khi chỉ tạo lab.
