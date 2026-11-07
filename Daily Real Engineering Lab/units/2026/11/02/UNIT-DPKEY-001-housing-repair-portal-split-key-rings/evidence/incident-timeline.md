# Incident timeline (synthetic, local simulation)

- 09:00: Replica A phát hành session ticket cho một operator.
- 09:01: Request trên A được chấp nhận.
- 09:02: Load balancer gửi request kế tiếp sang B; xác thực ticket thất bại.
- 09:03: Retry tới A lại thành công, database và CPU không có spike.
- 09:15: Restart A; request tiếp theo tới A vẫn hợp lệ.
- 09:20: Portal quản trị dùng cùng host nhưng khác application identity không đọc được ticket của portal chính.

## Hypothesis matrix
| Hypothesis | Expected discriminator |
|---|---|
| Clock skew | Thất bại tương quan thời gian/expiry thay vì replica |
| Sticky session / routing | Thất bại tương quan node đích; affinity chỉ che triệu chứng |
| Ticket material/configuration | Kiểm tra khả năng giải mã chéo node, restart và cách ly ứng dụng |

Các số liệu trên là tình huống mô phỏng, không phải production telemetry thật.
