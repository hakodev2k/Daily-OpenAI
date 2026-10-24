# Investigation guide

Bài tập tập trung vào correctness của một read-only API. Đừng mở solution trước khi chạy `reproduce.ps1`.

1. Ghi nhận các ID thực tế trong cold và warmed.
2. Xác minh `SaveChangesAsync` chỉ chạy lúc seed, không có write ở giữa hai read.
3. Bật EF Core query logging tại chỗ nếu cần; tránh đưa dữ liệu cá nhân vào log thực tế.
4. Quan sát `ChangeTracker.Entries<Enrollment>()` sau prior query; so sánh với context cold.
5. Đề xuất một fix tại read boundary và ghi trade-off với update workflows.
6. Chạy `verify.ps1`, bảo đảm kiểm tra missing cohort không bị mất.

**Phân biệt:** SQL đúng không bảo đảm object graph cuối cùng đúng. Bài học yêu cầu bằng chứng ở cả hai tầng.
