# Incident evidence — Ranger radio desk

**Giả lập:** 09:00, ca sáng có Lan và Minh đang trực; policy yêu cầu còn ít nhất 1 người.

| Timestamp (logical) | Request | Result |
|---|---|---|
| T0 | Lan xin nghỉ | Pending |
| T0 | Minh xin nghỉ | Pending |
| T1 | Lan xin nghỉ | Approved |
| T1 | Minh xin nghỉ | Approved |
| T2 | Dashboard | On-duty: 0 |

Các lần gửi tuần tự trong môi trường thử nghiệm không tái hiện trạng thái trên. Không có lỗi HTTP 500; audit ghi nhận cả hai quyết định là thành công. Các request được fixture đồng bộ ở một điểm cố định để tái hiện nhất quán, thay vì phụ thuộc timing của máy.

**Dữ liệu cần thu thập:** số approved, số còn trực, quyết định theo từng nhân viên, số revision nếu instrument thêm, và điểm kiểm tra policy. Chưa có bằng chứng cho thấy đây là lỗi của UI hay cache.
