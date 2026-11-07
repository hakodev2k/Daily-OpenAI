# Scenario & constraints
Gateway gửi thông báo cho site A, B, C. Site A có P1 rồi A2; site C có T1 rồi C2. Một số phản hồi là lỗi vĩnh viễn (recipient invalid), một số là lỗi tạm thời (503). Worker chỉ có 12 logical ticks cho một batch.

**Business contracts:** (1) thông báo độc lập ở site khác không bị chặn vô hạn; (2) thứ tự trong cùng site được giữ khi thông báo còn chờ; (3) không retry vô hạn; (4) thông báo bỏ qua phải truy vết được; (5) lỗi không phân loại không được nuốt im lặng.

**Nhiệm vụ thiết kế:** viết ADR ngắn cho lựa chọn giữa immediate retry, delayed bounded retry + dead-letter, và fail-fast toàn batch. Chỉ ra tiêu chí cho retryable/permanent, max attempts, thời gian chờ, redrive và alerting. Simulator dùng single-process; trong production phải đánh giá persistence, crash recovery, duplicate delivery và idempotency riêng.
