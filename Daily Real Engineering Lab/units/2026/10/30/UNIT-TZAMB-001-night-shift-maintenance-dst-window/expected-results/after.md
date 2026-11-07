# Sau refactor

Chạy `./verify.ps1`: tất cả 7 case PASS, exit code 0. `night-101` chuyển thành `2026-11-01T05:30:00+00:00`; `night-102` thành `2026-11-01T06:30:00+00:00`. Các input không đủ hoặc không nhất quán bị từ chối; lịch bình thường vẫn chạy.

Chạy `./reproduce.ps1` sau khi sửa sẽ không còn tái hiện baseline nên có thể trả exit code 1; đó là hành vi chủ đích của hai script riêng biệt.
