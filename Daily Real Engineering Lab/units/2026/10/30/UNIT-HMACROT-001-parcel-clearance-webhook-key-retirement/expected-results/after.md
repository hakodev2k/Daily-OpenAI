# After — measurable acceptance criteria

- Sau 09:30, **0** K1 webhook payload được xác thực, kể cả retry.
- E103 và E105 phải được phục hồi từ nguồn authoritative độc lập (hoặc từ retry được partner re-sign theo hợp đồng đã sửa), không dựa vào tin tưởng K1 sau cutoff.
- E101/E102/E104/E106 có đúng một canonical transition; duplicate E106 không tăng transition count.
- E107 và E108 vẫn bị reject; không dùng feed như cơ chế tự động hợp thức hóa payload không xác thực.
- Mỗi event được theo dõi theo eventId, digest, origin, firstSeenAt, processedAt và reconciliation cursor; sự kiện cùng ID khác digest bị cách ly.
- Proof cần bao gồm feed completeness, cursor recovery, 24h bound, 3-replica rollout, outage và alerting. Nếu thiếu proof, trạng thái phải là CONDITIONAL GO/NO-GO, không báo PASS.
