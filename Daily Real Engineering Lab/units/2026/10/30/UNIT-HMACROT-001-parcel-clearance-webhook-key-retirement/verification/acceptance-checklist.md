# Verification — tabletop acceptance gates

Đánh dấu bằng evidence; không được tự động đánh PASS vì đã vẽ sơ đồ.

- [ ] Phân biệt event delivery và business processing; tính đúng E103 retry 32 phút sau createdAt, E105 retry 23 giờ 39 phút sau createdAt.
- [ ] Chứng minh webhook-only không đồng thời đạt hard K1 cutoff và recovery mọi first-delivery K1 đến 24h.
- [ ] Không verify K1 sau 09:30; E107 signature invalid và E108 future skew vẫn reject.
- [ ] Không tin eventId/digest từ webhook chưa authenticated; không biến HTTP 401 thành trusted event.
- [ ] Inbox unique theo `(partnerId,eventId)`, kiểm tra digest mismatch, commit state + cursor đúng transaction/atomic boundary.
- [ ] Reconciliation qua OAuth/TLS có tenant scoping, cursor checkpoint, pagination, retry và gap detection.
- [ ] Test mất một page, cursor expired, duplicate feed/webhook, 3 replica restart và feed outage.
- [ ] Định nghĩa SLI: oldest unprocessed event age, reconciliation lag, cursor gap, invalid signature rate, K1 verification count after cutoff.
- [ ] Phân biệt partner's claimed 48h retention/2m delay với kết quả test; có điều kiện GO/NO-GO cụ thể.
- [ ] Có owner cho secret distribution, emergency revoke, rollback và incident communication.
- [ ] ADR so sánh A/B/C/D và ghi residual risk.
- [ ] Requirement change được xử lý mà không tự ý nới hard security cutoff.

**PASS** khi tất cả các mục có rationale và test evidence/plan phù hợp; **CONDITIONAL GO** nếu thiết kế hợp lý nhưng chưa có proof completeness; **NO-GO** nếu ràng buộc cứng không thể đáp ứng.
