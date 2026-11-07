# Incident snapshot — đường hầm T-17

| Case | DurationMs trên dashboard | CPU worker | Queue depth |
|---|---:|---:|---:|
| stable | 250 | 8% | 0 |
| backward-step | -29750 | 9% | 0 |
| forward-step | 45250 | 8% | 0 |

- Cả ba tác vụ có cùng lượng công việc, cùng elapsed thực tế trong fixture.
- Không có retry, exception, tăng CPU hoặc queue depth.
- Audit bắt buộc giữ UTC start/end thực, không làm đẹp timestamp.
- Đây là dữ liệu mô phỏng xác định, không phải log production.

Hãy viết hai giả thuyết và phép thử bác bỏ trước khi mở hints.
