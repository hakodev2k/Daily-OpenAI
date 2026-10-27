# Before (starter nguyên bản)
- `./reproduce.ps1` exit 0 vì chứng minh được triệu chứng.
- `EVENT host-startup-before-release=False`.
- Gate được mở sau khi kiểm tra; `warmup-completed` và `host-stopped` vẫn xuất hiện.
- `./verify.ps1` exit 1 nếu source chưa sửa, với `FAIL: verify contract violated.`

Không dùng exit 0 của reproduce làm bằng chứng đã fix.
