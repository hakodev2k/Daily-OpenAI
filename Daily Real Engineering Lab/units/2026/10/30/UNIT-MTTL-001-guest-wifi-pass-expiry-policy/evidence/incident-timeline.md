# Timeline điều tra (UTC, dữ liệu tổng hợp)

| Time | Observation | Gateway result | Storage observation |
|---|---|---|---|
| 14:50:00 | Cấp thẻ GUEST-72, expiresAt=15:00:00 | — | Document created |
| 14:54:59 | Kiểm tra khách còn hiệu lực | ALLOW | Document present |
| 14:55:00 | Nhân viên thu hồi một thẻ khác, ghi nhận thành công | — | revokedAt persisted |
| 14:55:20 | Thẻ đã thu hồi vẫn được kiểm tra | ALLOW | Document present |
| 15:00:00 | Kiểm tra đúng mốc expiry của GUEST-72 | ALLOW | Document present |
| 15:00:30 | Khách thử lại | ALLOW | Document present |
| 15:02:00 | Lượt dọn nền hoàn tất | — | Document absent |
| 15:02:05 | Khách thử lại | DENY | Document absent |

Dữ liệu nhiễu: p95 gateway 22–29 ms, CPU 18–24%, không có lỗi HTTP 5xx trong khoảng sự cố. Không có thay đổi code lúc 15:02.

Đặt câu hỏi: Điều kiện nào thực sự quyết định ALLOW? Dữ liệu nào vẫn đúng dù chưa bị xóa?
