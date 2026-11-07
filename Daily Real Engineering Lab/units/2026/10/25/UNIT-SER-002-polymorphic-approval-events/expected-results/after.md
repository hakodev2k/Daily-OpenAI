# After
- JSON có $eventType cùng derived fields.
- Replay khôi phục ApprovalGranted và ApprovalRejected với đúng giá trị.
- Discriminator lạ bị từ chối bằng JsonException.
- Verify: exit 0, VERIFIED.
- Reproduce sau fix: exit khác 0 vì triệu chứng cũ đã hết.
- Không cần so sánh thời gian chạy.
