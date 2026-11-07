# Synthetic incident timeline (không phải log production)

- 09:00: traffic chủ yếu vào replica A, khách truy cập báo phiên ổn định.
- 09:05: triển khai replica B, load balancer phân phối traffic giữa A/B.
- 09:06: cùng session token, A chấp nhận nhưng B từ chối.
- 09:10: restart A; phiên vừa phát hành cũng bị từ chối ở A mới.
- 09:12: database và ticket issuer healthy; không phát hiện payload bị thay đổi.

Dùng script để xác nhận hay bác bỏ các giả thuyết. Timeline chỉ là dữ kiện điều tra, không tự giải thích cơ chế.
