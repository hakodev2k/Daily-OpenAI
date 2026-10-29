# Incident evidence — dữ liệu mô phỏng

Bảng dưới đây mô tả trạng thái của **starter nguyên gốc**, không phải log production. Chạy `./run.ps1` để quan sát cùng các bộ đếm.

| Time | Event | Pending | InFlight | Completed | Artifacts |
|---|---|---:|---:|---:|---:|
| T+0 | BADGE-042 được nhận để render | 0 | 1 | 0 | 0 |
| T+1 | Worker nhận tín hiệu dừng giữa render | 0 | 1 | 0 | 0 |
| T+2 | Task kết thúc với cancellation | 0 | 0 | 1 | 0 |
| T+3 | Worker mới thử nhận job | 0 | 0 | 1 | 0 |
| U+0 | BADGE-077 được nhận để render | 0 | 1 | 0 | 0 |
| U+1 | Artifact đã ghi; gateway ném exception | 0 | 0 | 1 | 1 |
| U+2 | Worker mới thử nhận job | 0 | 0 | 1 | 1 |

Các số liệu được cố ý tối giản: không có network, database hay real broker. Cần suy luận về **ý nghĩa của trạng thái** trước khi chọn cách sửa. `PublishAttempts` là số lần thử ghi artifact, còn `Artifacts` là số artifact có key duy nhất.
