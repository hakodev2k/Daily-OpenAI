# Incident evidence (synthetic, deterministic)

| Case | HTTP headers | Body delay injected | Caller cancellation | Worker budget |
|---|---|---:|---:|---:|
| FAST | 200 immediately | 15 ms | none | 250 ms |
| SLOW_BODY | 200 immediately | 1200 ms | none | 250 ms |
| CALLER_CANCEL | 200 immediately | 1200 ms | after 90 ms | 3000 ms |

- Fake partner trả headers ngay từ `HttpMessageHandler`; body được cấp bởi `StreamContent`.
- Một request 200 không bảo đảm toàn bộ body đã được đọc.
- `HttpClient.Timeout` trong harness là 250 ms; bảng trên không khẳng định timeout đó áp dụng đến hết body.
- Chỉ số CPU thấp; không có DNS, socket, proxy, TLS, database hay Azure trong simulation.
- Đường dẫn `/fast` và `/slow` dùng cùng payload, khác độ trễ của lần đọc body đầu tiên.
- Kết quả thời gian thực tế phụ thuộc máy; ngưỡng kiểm chứng có biên để tránh dựa vào timing tuyệt đối.

Evidence này là dữ liệu mô phỏng, không phải telemetry production thật.
