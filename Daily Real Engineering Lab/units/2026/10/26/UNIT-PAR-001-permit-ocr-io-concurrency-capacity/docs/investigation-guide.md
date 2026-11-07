# Investigation guide
1. Chạy reproduce.ps1 và lưu exit code cùng metrics.
2. Ước lượng số đợt OCR tối thiểu cho 24 hồ sơ với 120 ms mỗi request.
3. So sánh hypothesis CPU saturation, partner throttling, thiếu concurrency, lỗi dữ liệu.
4. Đối chiếu requestedConcurrency và effectiveConcurrency, truy nguồn policy.
5. Dự đoán metrics khi N=3, 8, 16 trước khi thay đổi code.
6. Chạy verify.ps1 trên starter đã sửa; nếu fail, kiểm tra metrics từng cấu hình.
