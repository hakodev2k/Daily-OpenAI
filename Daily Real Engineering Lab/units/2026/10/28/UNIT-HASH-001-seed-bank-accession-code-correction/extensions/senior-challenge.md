# Senior challenge
Giả sử registry được chia sẻ cho 16 request song song và phải đồng bộ với PostgreSQL. Đề xuất atomic rename có unique constraint, xử lý conflict/rollback, audit event, optimistic concurrency và retry idempotency. Viết thêm một stress test chứng minh không có duplicate hay mất index dưới concurrency; so sánh lock trong memory với transaction ở database.
