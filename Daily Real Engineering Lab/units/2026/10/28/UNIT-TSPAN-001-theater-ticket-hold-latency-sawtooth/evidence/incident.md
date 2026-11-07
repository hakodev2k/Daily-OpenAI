# Evidence — Ticket-hold gateway (synthetic, deterministic)

Bản ghi sau mô phỏng dữ liệu từ một instance trong ca trực 20:00–20:10. Các duration đã được đo ở upstream call boundary; dữ liệu không phải số liệu production thật.

| Correlation | HTTP | Duration (ms) | Budget (ms) | Alert emitted |
|---|---:|---:|---:|---|
| TH-120 | 200 | 620 | 750 | no |
| TH-121 | 200 | 749 | 750 | no |
| TH-122 | 200 | 750 | 750 | no |
| TH-123 | 200 | 751 | 750 | yes |
| TH-124 | 200 | 900 | 750 | yes |
| TH-125 | 200 | 1200 | 750 | no |
| TH-126 | 200 | 1900 | 750 | yes |
| TH-127 | 200 | 2500 | 750 | no |

Telemetry khác cùng thời điểm (synthetic): CPU ~27%, queue depth 0, downstream status 200 cho cả 8 request; clock synchronization báo healthy. Các tín hiệu này không tự chứng minh nguyên nhân.

## Investigation prompts
- Hãy vẽ expected/actual cho mỗi sample.
- Kiểm tra quy tắc *strictly greater than* ở điểm bằng ngưỡng.
- Một giả thuyết có giải thích được đồng thời 751, 1200, 1900 và 2500 ms không?
- Vì sao HTTP 200 không chứng minh gateway đã tuân thủ SLA?

Không sử dụng log này như chứng cứ về một hệ thống thật; hãy xác nhận bằng `reproduce.ps1`.
