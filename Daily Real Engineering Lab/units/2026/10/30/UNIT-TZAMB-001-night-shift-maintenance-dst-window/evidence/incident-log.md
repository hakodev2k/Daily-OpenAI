# Evidence snapshot — dữ liệu giả lập

| Request | LocalStart (New York) | Offset khai báo | UTC đối tác dự kiến | Quan sát ở starter |
|---|---|---|---|---|
| night-101 | 2026-11-01 01:30 | -240 | 05:30Z | 06:30Z |
| night-102 | 2026-11-01 01:30 | -300 | 06:30Z | 06:30Z |
| night-103 | 2026-11-01 01:30 | null | REJECT | 06:30Z |
| spring-201 | 2026-03-08 02:30 | -300 | REJECT | REJECT |
| day-301 | 2026-11-01 03:30 | -300 | 08:30Z | 08:30Z |
| day-302 | 2026-11-01 03:30 | -240 | REJECT | 08:30Z |
| day-303 | 2026-10-31 14:00 | null | 18:00Z | 18:00Z |

Dữ liệu được cố định để reproduction không phụ thuộc ngày chạy hoặc đồng hồ máy. Một số case thành công là negative control cho giả thuyết lỗi.
