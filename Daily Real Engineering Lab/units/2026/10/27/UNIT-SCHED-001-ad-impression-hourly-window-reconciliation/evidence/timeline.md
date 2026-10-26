# Evidence fixture (kết quả dự kiến, không phải log đã thực thi)

Mốc khởi tạo: `2026-10-27T00:00:00+07:00`. Kích thước cửa sổ: 1 giờ. Tại wake-up, logical clock đã tới `05:00+07:00`.

| Dữ kiện | Kỳ vọng nghiệp vụ | Starter dự kiến |
|---|---:|---:|
| Cửa sổ đến hạn | 5 | 5 |
| Sink call sau một wake-up | 5 | 1 |
| Checkpoint | 05:00 | 01:00 |
| Exception | không | không |

**Bằng chứng nhiễu:** timer vẫn đang chạy, tiến trình không crash, và một bản ghi settlement có thể đã được tạo. Những dữ kiện này không chứng minh đã xử lý đủ giờ.

**Thử lỗi:** sink được cấu hình thất bại tại 03:00. Checkpoint đúng sau lần gọi thất bại phải là 02:00; lần retry phải xử lý 03:00, 04:00, 05:00.