# Incident timeline — synthetic

09:00:00 — Region X1 mất traffic. 09:01:40 — On-call xác nhận region outage.
09:04:00 — Dashboard p95 replica lag hiển thị 52 giây và màu xanh.
09:05:20 — Kiểm tra durable replay của drill C3 cho thấy lag tại failure là 425 giây.
09:08:00 — Operator đề xuất promote ngay để đạt RTO.
09:10:00 — Chưa có bằng chứng primary cũ đã được fence.
09:27:00 — Site X2 phục vụ write sau kiểm tra thủ công trong drill C3.
09:45:00 — Product đề xuất công bố RPO 5 phút/RTO 30 phút.

Nhiễu: CPU primary 42%, edge cache hit 91%, một lỗi DNS dịch vụ gửi email ngoài đường commit.

Hãy phân biệt bằng chứng quyết định với số liệu không liên quan và chỉ ra thông tin còn thiếu.
