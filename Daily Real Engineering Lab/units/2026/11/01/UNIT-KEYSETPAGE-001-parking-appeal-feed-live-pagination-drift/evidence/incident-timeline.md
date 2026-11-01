# Timeline và evidence (synthetic, deterministic)

| UTC | Event | Evidence |
|---|---|---|
| 12:06:00.000 | GET page 1 | 200; count=3; IDs=106,105,104; response=8 ms |
| 12:06:00.090 | POST new appeal | ID=107; CreatedAt later than all existing rows |
| 12:06:00.160 | GET page 2 | 200; count=3; IDs=104,103,102; response=7 ms |
| 12:06:00.500 | Shift audit | seen unique IDs=5, expected original IDs=6 |
| 12:07:00.000 | SQL health check | connections healthy; CPU low; no deadlock |

Các giá trị thời gian là fixture mô phỏng, không phải telemetry production thật.

## Bằng chứng có thể đánh lạc hướng
- Một deploy UI mới diễn ra đầu ca nhưng không đổi sort order.
- Log không có 5xx hoặc timeout.
- Mỗi response có đúng ba item, nên test chỉ assert page size vẫn xanh.

## Nhiệm vụ điều tra
Đưa ít nhất ba hypothesis: backend sort, frontend merge/dedupe, và sự thay đổi dữ liệu giữa các request. Thiết kế một phép thử để phân biệt chúng. Kiểm tra tiếp trường hợp xóa bản ghi phía trước cursor và timestamp bằng nhau.
