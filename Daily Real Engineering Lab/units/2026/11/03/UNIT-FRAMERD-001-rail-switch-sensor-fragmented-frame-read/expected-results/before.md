# Before
- `./run.ps1` in ra `SWITCH-17|POSITION=NORMAL`.
- `./reproduce.ps1` in `REPRODUCED` và exit code 0 vì baseline đọc frame liên tục thành công nhưng frame phân mảnh thất bại.
- `./verify.ps1` trước khi sửa trả exit code 1, dừng ở case `fragmented header and payload`.
- Không có dữ liệu byte nào bị thay đổi trong fixture.
