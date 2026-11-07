# Senior Challenge — rollout an toàn
1. Đề xuất shadow-evaluation trong 14 ngày: chạy policy mới song song policy cũ, không gửi pager mới.
2. Thiết kế event schema ghi `policyVersion`, `sloTarget`, `burn5m`, `burn30m`, `decision`, `reason`.
3. Tạo thêm fixture missing 8 bucket và burst chỉ 2 request; phân biệt missing telemetry, low sample và genuine outage.
4. Viết ADR về rollback, duplicate suppression, alert grouping, ticket vs page và ownership.
5. Giải thích vì sao metric counter reset và late-arriving buckets có thể làm rolling window sai lệch.
