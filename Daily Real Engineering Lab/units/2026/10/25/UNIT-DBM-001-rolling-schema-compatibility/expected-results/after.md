# Expected results — acceptance gates

Một phương án được chấp nhận khi chứng minh được:
1. Mọi app version **được phép** chạy tại từng phase có cả read/write contract hợp lệ.
2. Không có dữ liệu hiển thị cũ sau một write đã commit do hai cột lệch nhau trong rollout.
3. Backfill batch có thể retry, được theo dõi tiến độ, không yêu cầu downtime.
4. Rollback ứng dụng trong 24 giờ (sau requirement change: 72 giờ) vẫn đọc/ghi đúng, không restore DB.
5. Drop legacy column chỉ xảy ra sau compatibility, consumer và rollback gates; nếu chưa đủ bằng chứng, hoãn contract.
6. Tests bao phủ mixed versions, race với backfill, RowVersion conflict, tenant isolation và rollback.
7. Quyết định có so sánh complexity, SQL locking, release duration, telemetry và ownership.

Không có ngưỡng latency tuyệt đối bảo đảm trên mọi máy; mục tiêu là giữ p95 trong SLO đã nêu và không tạo lock contention đáng kể.
