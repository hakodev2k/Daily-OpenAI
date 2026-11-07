# Evidence packet — synthetic expected observations

Đây là **dữ liệu mô phỏng dự kiến từ starter** qua static review, không phải log thu từ một lần thực thi thật. Dùng `reproduce.ps1` để xác nhận trên máy.

| Local interview date | Expected UTC | Starter UTC | Result |
|---|---|---|---|
| 2026-03-07 | 14:00Z | 14:00Z | pass |
| 2026-03-08 | 13:00Z | 14:00Z | fail |
| 2026-03-09 | 13:00Z | 14:00Z | fail |
| 2026-10-31 | 13:00Z | 13:00Z | pass |
| 2026-11-01 | 14:00Z | 13:00Z | fail |
| 2026-11-02 | 14:00Z | 13:00Z | fail |

Điều tra theo nhóm: dữ liệu đầu vào; timezone ID; local-to-UTC conversion; việc lưu UTC; thời điểm gửi thực tế. Không kết luận nguyên nhân chỉ từ triệu chứng.
