# Synthetic incident evidence

| Event | Observation |
|---|---|
| 09:14:00 | Caption job start, converter child launched |
| 09:14:01 | Parent waiting, no caption result |
| 09:14:03 | Job timeout; watchdog stops child |
| 09:14:04 | Slot released, no caption artifact |

CPU thấp, không có network request hay database write. Tăng deadline đơn thuần trong mô phỏng không giải quyết triệu chứng. Dữ liệu synthetic, không phải log đã thực thi.
