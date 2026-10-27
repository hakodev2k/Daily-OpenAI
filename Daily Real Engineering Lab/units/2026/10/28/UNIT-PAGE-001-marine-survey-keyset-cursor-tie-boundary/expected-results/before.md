# Before — baseline
- `./reproduce.ps1` in `SOURCE_COUNT=6`, `RECEIVED_COUNT=5`, `MISSING_IDS=obs-c`.
- `REPRODUCTION_CONFIRMED` nghĩa là lỗi đã được reproduce có kiểm soát.
- `./verify.ps1` trên starter chưa sửa phải thất bại ở regression `page-size-1` hoặc `page-size-2`.
- Không có network, database hoặc timing ngẫu nhiên; không cần đo latency.
