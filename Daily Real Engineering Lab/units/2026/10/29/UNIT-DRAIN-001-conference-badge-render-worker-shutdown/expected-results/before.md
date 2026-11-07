# Before — starter nguyên gốc

Chạy `./reproduce.ps1` trước khi sửa. Kết quả mong đợi là `EXPECTED_FAILURE_REPRODUCED` với exit code 0, và:
- BADGE-042: trước/sau retry `Pending=0, InFlight=0, Completed=1, Artifacts=0, PublishAttempts=0`.
- BADGE-077: trước/sau retry `Pending=0, InFlight=0, Completed=1, Artifacts=1, PublishAttempts=1`.
- Đây là **đã chứng minh lỗi**, không phải trạng thái đúng.

Nếu khác, chạy `dotnet --version`, xác nhận SDK 8.x, không chỉnh `Program.cs` và thử lại từ starter nguyên gốc.
