# After — contract đã sửa

verify.ps1 phải trả exit code 0; tất cả bảy case PASS. Đặc biệt:
- Ngay tại ExpiresAtUtc: DENY.
- Document chưa bị cleanup nhưng đã hết hạn: DENY.
- Đã thu hồi và RevokedAtUtc <= nowUtc: DENY.
- Chưa đến thời điểm thu hồi có hiệu lực: ALLOW nếu còn hạn.
- Không tồn tại document: DENY.

Đây là contract test cục bộ, không thay thế integration test với MongoDB, latency load test hoặc chứng minh consistency trên hai replica.
