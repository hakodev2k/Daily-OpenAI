# Incident evidence — sanitized local fixture

| Server receipt order | Device | Change ID | Field | Base field revision | Client timestamp (+07) | Proposed value |
|---|---|---|---|---:|---|---|
| 1 | A | A-101 | Status | 0 | 10:05 | InProgress |
| 2 | B | B-201 | TechnicianNote | 0 | 10:03 | Valve seal damaged |
| 3 | B | B-202 | Status | 0 | 10:07 | Cancelled |
| 4 | B | B-201 | TechnicianNote | 0 | 10:03 | Valve seal damaged |
| 5 | A | A-102 | Status | 1 | 10:01 | Completed |

Số liệu giả lập: thời điểm server nhận là thứ tự 1→5; client timestamp không bảo đảm đồng bộ NTP. Mỗi thiết bị đã đọc bản Work Order trước khi mất kết nối. Mọi thay đổi được gửi qua cùng một endpoint; không có sự cố mạng ở thời điểm server xử lý.

**Quan sát cần thu thập:** Kết quả từng lần Apply, trạng thái cuối của hai field, field revisions, số lần Revision tăng. Đối chiếu với acceptance contract, không suy luận từ tên enum đơn thuần.
