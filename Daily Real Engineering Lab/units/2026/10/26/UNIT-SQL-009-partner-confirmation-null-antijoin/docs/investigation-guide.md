# Investigation guide

Không xem solution trước khi thu thập bằng chứng.

1. Chạy `./scripts/reproduce.ps1`; lưu output của `TABLE lab.missing_enrollments;`.
2. Truy vấn `SELECT tenant_id, status, count(*) FROM lab.enrollments GROUP BY 1,2 ORDER BY 1,2;`.
3. Truy vấn `SELECT tenant_id,batch_id,enrollment_id FROM lab.partner_confirmations ORDER BY confirmation_id;`.
4. Kiểm tra đối chứng theo từng enrollment, chỉ rõ tenant/batch/status.
5. Viết ít nhất ba hypothesis và một SQL query để bác bỏ mỗi hypothesis.
6. Sửa `starter/query.sql`, chạy `./scripts/verify.ps1`, ghi kết quả.
7. Thử thêm các xác nhận từ tenant khác và batch cũ; đánh giá cách mở rộng test khi dữ liệu có duplicate và NULL.

**Lưu ý:** `reproduce.ps1` luôn dùng `scripts/original-query.sql` (baseline), còn `verify.ps1` dùng file người học sửa trong `starter/`.
