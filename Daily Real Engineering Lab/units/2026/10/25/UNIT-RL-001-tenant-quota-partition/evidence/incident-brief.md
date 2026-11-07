# Incident brief — dữ liệu tình huống giả lập

**Nguồn:** fixture mô phỏng, không phải log đã đo trên production hay kết quả chạy thực tế của tác giả.

Hợp đồng sản phẩm: mỗi authenticated tenant có tối đa 2 requests / 5 phút trên tất cả endpoint; anonymous dùng một ngân sách dự phòng riêng. Không queue; vượt ngân sách trả về 429.

Báo cáo từ vận hành:
- 10:00:01: Alpha gọi `/api/render`, nhận 200.
- 10:00:02: Alpha gọi `/api/render`, nhận 200.
- 10:00:03: Beta gọi `/api/render`, nhận 429 dù là request đầu.
- 10:01:00: Alpha gọi `/api/status`, nhận 200 dù theo hợp đồng đã hết quota.
- Auth logs: tenant claim của Alpha và Beta đều hợp lệ.
- CPU thấp; không có dependency timeout; request không đi vào xử lý tài liệu khi bị 429.

Các dòng trên là **symptom được đưa vào bài toán**, không phải kết luận root cause. Hãy dùng harness để tạo evidence có thể kiểm tra lại. Cần phân biệt: lỗi ngưỡng, lỗi middleware, lỗi identity, lỗi lựa chọn partition và lỗi xử lý time window.

Giới hạn: harness gọi limiter trực tiếp nên không xác nhận status code HTTP hay hành vi middleware; `DENY` trong harness tương ứng lease không được cấp, không tự động là HTTP 429.
