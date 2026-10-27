# Incident evidence — synthetic fixture expectations

Đây là **kỳ vọng**, không phải kết quả chạy thực tế trong phiên tạo lab.

| Quan sát | Trước fix | Sau fix |
|---|---|---|
| Report response | HTTP 200, X-Export-State: ready, body chưa kết thúc | Không đổi |
| ReportRequests | 1 | 1 |
| HealthRequests | 0 trước deadline | 1 |
| Health result | Timeout khoảng 900ms | HTTP 200 / PONG |

Hãy phân biệt queue wait ở client với latency phía origin. Nếu không reproduce, kiểm tra .NET 8, loopback 127.0.0.1, fake server và mã starter chưa bị sửa. Không tăng timeout để làm test xanh.
