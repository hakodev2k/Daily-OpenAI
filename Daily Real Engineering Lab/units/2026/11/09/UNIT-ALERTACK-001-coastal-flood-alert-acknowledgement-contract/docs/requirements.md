# Yêu cầu và ràng buộc
- 240 gateway; mỗi gateway điều khiển 1–4 còi và có cảm biến phản hồi relay (không chứng minh âm thanh thực sự).
- Broker at-least-once; duplicate, out-of-order và mất kết nối có thể xảy ra.
- P95 mạng thường 2 giây; outage tối đa 30 phút; độ lệch đồng hồ gateway có thể ±90 giây.
- Lệnh ALERT / ALL_CLEAR được phát theo vùng; trung tâm có thể phát phiên bản lệnh mới trong khi phiên bản cũ còn trên broker.
- Không được phát lại lệnh ALERT đã hết hiệu lực sau ALL_CLEAR; một trạm offline không được tự báo thành công.
- Dashboard phải phân biệt dispatched, gateway accepted, actuator confirmed, unknown và failed.
- Người vận hành phải có thể thấy vùng thiếu xác nhận trong 10 giây kể từ khi mất heartbeat.
- Có đường vận hành tại chỗ (local manual override) và quy trình xác minh ngoài băng cho các khu vực rủi ro cao.
- Chi phí: tối đa 2 VM broker + 1 database vận hành; đội on-call 2 người, không thêm hệ thống cloud bắt buộc.
- Dữ liệu audit giữ 90 ngày; không lưu dữ liệu cá nhân.
- Thiết kế phải có test cho duplicate, reorder, reconnect, command expiry, relay failure, clock skew và lost ACK.

## Requirement change
Sau khi bạn chốt thiết kế ban đầu, giả định một vùng không có Internet 30 phút nhưng cảm biến mực nước tại trạm vẫn hoạt động. Hãy xác định ai được phép kích hoạt còi cục bộ, cách chống conflict với lệnh trung tâm và cách ghi audit khi kết nối trở lại.

## Deliverables
1. State machine có invariant rõ ràng.
2. JSON contract mẫu: command, gateway acceptance ACK, actuation ACK và heartbeat.
3. Thứ tự xử lý và quy tắc expiry/replay.
4. Monitoring / escalation / manual override.
5. Phương án kiểm thử bằng timeline; ADR ngắn có alternatives và trade-offs.