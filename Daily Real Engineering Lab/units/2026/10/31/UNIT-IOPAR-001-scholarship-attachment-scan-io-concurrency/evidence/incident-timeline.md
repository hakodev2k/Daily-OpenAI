# Synthetic evidence — không phải production telemetry

| Mốc | Tín hiệu | Cần kiểm chứng |
|---|---|---|
| 01:00 | Batch 24 tệp bắt đầu | Input |
| 01:01 | CPU thấp | Có thật sự CPU-bound? |
| 01:02 | Downstream latency ổn định | Có lỗi mạng không? |
| 01:03 | Không có 429 với quota 8 | Còn headroom? |
| 01:04 | Peak in-flight thấp | Admission policy? |
| 01:05 | Partner B có quota 1 | Rủi ro nếu fan-out vô hạn? |

Số đo thực nghiệm lấy từ scripts/run.ps1. Dòng CPU/mạng chỉ là dữ kiện giả lập phục vụ điều tra, không do simulator thu thập.
