# Before — starter gốc
- `initial-policy` và `reject-nonpositive` PASS.
- `reduced-quota`: configuredMax=2, requested=3, expected=False nhưng actual=True.
- `reduced-quota-allowed` PASS.
- `increased-quota`: configuredMax=5, requested=5, expected=True nhưng actual=False.
- `increased-quota-exceeded` PASS.
- Kết thúc bằng `CONFIG_DRIFT_DETECTED`, exit code 23.
- `reproduce.ps1` chuyển triệu chứng trên thành `REPRODUCED` và exit 0.
Đây là kết quả dự kiến từ static review, không phải log thực thi đã xác nhận.
