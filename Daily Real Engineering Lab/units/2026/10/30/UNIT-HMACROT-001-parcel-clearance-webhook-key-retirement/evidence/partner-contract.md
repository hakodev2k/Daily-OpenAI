# Synthetic partner contract — Version 2026-10

- `X-Partner-Key-Id`: `K1` hoặc `K2`.
- `X-Partner-Timestamp`: ISO-8601 UTC instant; trong bảng CSV hiển thị local +07 để dễ so sánh.
- `X-Partner-Event-Id`: ổn định trên mọi retry.
- `X-Partner-Signature`: HMAC-SHA256 trên `timestamp + "." + rawBody`, dùng key tương ứng; so sánh constant-time.
- Retry giữ nguyên timestamp, raw bytes, keyId, signature, eventId; tối đa 24h từ createdAt. Retry không được xem là event mới.
- Partner có `GET /v1/events?cursor=...` qua TLS + OAuth2 client credentials, giữ event tối thiểu 48h, thông thường cập nhật sau <=2 phút. Có pagination và cursor opaque; chưa cung cấp bằng chứng không thiếu event khi phân trang, failover hay reconnect.
- Không có cam kết partner re-sign retry bằng khóa mới trong hợp đồng hiện hành.
- Không được suy ra authenticity chỉ từ `eventId` trong HTTP request chưa được xác thực.
- Feed là nguồn authoritative độc lập theo tuyên bố đối tác, nhưng phải chứng minh source-of-truth semantics và scope theo tenant trước khi GO.

## Các điểm cần xác minh với partner
Có thể lấy lại payload đầy đủ bằng feed không? Cursor có snapshot/ordering guarantee không? Feed có bao gồm event từng thất bại webhook không? Có pagination limit/rate limit? Partner có thể re-sign retries trong tương lai không? Ai sở hữu incident reconciliation?
