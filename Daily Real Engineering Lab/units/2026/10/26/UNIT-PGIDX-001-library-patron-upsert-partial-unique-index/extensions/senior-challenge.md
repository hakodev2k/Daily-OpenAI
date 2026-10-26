# Senior Challenge — Migration rollout

Thiết kế thứ tự migration, deploy, rollback khi một version ứng dụng cũ vẫn gửi conflict target không có predicate. Xác định điểm không thể rollback an toàn. Đề xuất kiểm thử trên PostgreSQL thật trong CI và test concurrent upsert ở hai worker. Phân tích khác biệt giữa partial unique index và UNIQUE CONSTRAINT.
