# Incident Snapshot

Dữ liệu dưới đây mô phỏng một production burst đã được thu gọn để điều tra local.

```text
16:01:12.004 export-A accepted   active=1 availableSlots=1
16:01:12.011 export-B accepted   active=2 availableSlots=0
16:01:12.128 client-A disconnected
16:01:12.131 client-B disconnected
16:01:12.286 sampler             active=2 availableSlots=0
16:01:12.290 cpu                 14%
16:01:12.291 gc-gen2             0 collections/min
16:01:13.214 export-A finished   active=1 availableSlots=1
16:01:13.220 export-B finished   active=0 availableSlots=2
```

## Câu hỏi điều tra

- Tại sao capacity vẫn bằng 0 sau khi cả hai client đã rời đi?
- CPU thấp có loại trừ capacity exhaustion không?
- Operation timeout và request lifetime có đang biểu diễn cùng một policy hay hai policy khác nhau?
