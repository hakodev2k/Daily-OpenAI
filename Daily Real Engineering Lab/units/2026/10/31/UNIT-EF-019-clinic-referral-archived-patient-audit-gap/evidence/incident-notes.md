# Incident evidence — synthetic, cần đối chiếu khi chạy
- 09:00: tác vụ xuất lịch sử chuyển tuyến cho `TENANT-ALPHA` nhận HTTP 200 nhưng thiếu một sự kiện.
- 09:04: DBA xác nhận hai `ReferralEvents` thuộc tenant còn trong bảng.
- 09:07: nhóm vận hành thấy một `Patient` đã chuyển trạng thái archived trước đó.
- 09:09: log hệ thống có một truy vấn health-check chậm (có thể là nhiễu, không có bằng chứng liên quan).
- 09:12: CPU ổn định; không có retry hoặc timeout.
- Cần thu thập: SQL từ `ToQueryString()`, `Stored events`, `Exported rows`, `IDs`.
- Không có log production thật được cung cấp; đây là fixture dựng để điều tra, không phải dữ liệu bệnh nhân thật.
