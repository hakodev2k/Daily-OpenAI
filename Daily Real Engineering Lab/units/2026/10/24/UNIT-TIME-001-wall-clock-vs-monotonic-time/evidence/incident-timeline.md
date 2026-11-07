# Synthetic incident evidence

Dữ liệu mô phỏng, không phải production telemetry. Hãy phân loại signal/noise trước khi đọc hints.

| Event | Elapsed từ simulator | Watchdog | Context |
|---|---:|---|---|
| Batch A checkpoint | 4 s | expired | host time sync xảy ra trong batch |
| Batch B checkpoint | 12 s | active | host time sync xảy ra trong batch |
| Batch C checkpoint | 10 s | expired | không có adjustment |

Giả thuyết cạnh tranh: CPU starvation, delayed checkpoint, timeout policy mismatch, time source không phù hợp. Đề xuất phép đo loại trừ từng giả thuyết. Timestamp của log không tự chứng minh elapsed duration.
