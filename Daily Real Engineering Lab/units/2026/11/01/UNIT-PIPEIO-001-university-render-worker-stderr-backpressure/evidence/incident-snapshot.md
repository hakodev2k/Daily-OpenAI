# Incident snapshot (synthetic fixture, NOT executed logs)

| Relative time | Signal | Observation |
|---|---|---|
| T+0ms | wrapper | child spawned, stdout/stderr redirected |
| T+20ms | renderer | starts diagnostic payload to stderr |
| T+~4s | wrapper | deadline exceeded, child terminated |
| T+~4s | wrapper | DEADLOCK_REPRODUCED, exit 23 |
| control run | wrapper | 128-character stderr completes, exit 0 |

Mốc thời gian chỉ minh họa, không phải đo thực tế. Dữ liệu thực lấy từ reproduce.ps1. Phân biệt các giả thuyết: renderer CPU-bound, lỗi nội dung bảng điểm, giới hạn process/stream I/O và timeout quá thấp.
