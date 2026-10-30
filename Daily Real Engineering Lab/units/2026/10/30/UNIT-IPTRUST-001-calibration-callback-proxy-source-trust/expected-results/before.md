# Before
- `./reproduce.ps1` exit 0 vì fixture `spoofed-direct` bị chấp nhận trái policy.
- `./run.ps1` hiển thị một số `MISMATCH` (đặc biệt request direct và multi-hop).
- `./verify.ps1` exit 1; chưa được coi là đã sửa.
- Số lượng mismatch là xác định, không phụ thuộc tốc độ máy.
