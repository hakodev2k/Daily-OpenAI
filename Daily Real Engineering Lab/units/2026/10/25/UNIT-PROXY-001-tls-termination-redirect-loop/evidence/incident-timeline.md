# Incident timeline (mô phỏng; chưa chạy thực tế)

| Mốc | Evidence | Nhận xét ban đầu |
|---|---|---|
| 09:00 | Deploy bản API mới, edge vẫn terminate TLS | Certificate còn hạn 61 ngày |
| 09:05 | Edge access log: GET /status → upstream 307, `Location: https://partner.example.test/status` | Browser theo redirect và quay lại edge |
| 09:07 | Edge counter: redirect ratio 91%, upstream 5xx < 0.1% | Không có dấu hiệu exception backend |
| 09:10 | Kestrel: listening on HTTP 127.0.0.1:5077 | Upstream giữa edge và app dùng HTTP |
| 09:12 | DNS lookup và handshake TLS tại edge đều thành công | Nghi vấn DNS/certificate yếu đi |
| 09:14 | Proxy header contract: `X-Forwarded-Proto: https` | Cần xác minh app xử lý contract ra sao |
| 09:16 | CPU app ~7%, memory ổn định | Không giống quá tải |

Các số liệu là fixture điều tra; không được coi là telemetry của hệ thống thật. `probe/Program.cs` tạo evidence thực tế khi chạy local.
