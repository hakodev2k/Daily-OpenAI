# Incident timeline — dữ liệu mô phỏng

Bảo tàng ghi nhận restart tăng khi đối tác chấm điểm rủi ro gián đoạn. Đây là fixture, không phải log production.

| Pha | Đối tác | /health/live | /health/ready | Process |
|---|---|---|---|---|
| 09:00 | Available | 200 | 200 | Responding |
| 09:05 | Unavailable | 503 | 503 | Responding |
| 09:10 | Available | 200 | 200 | Responding |

Giả thuyết cần phân biệt: process deadlock, lỗi bind port, dependency timeout, lựa chọn tập health checks, Kubernetes probe threshold. Fixture local chỉ xác nhận endpoint status, không đo restart thật.
