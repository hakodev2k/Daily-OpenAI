# After — learner sửa starter

Chạy `./verify.ps1` trên code trong `starter/`. Kết quả mong đợi là `VERIFICATION_PASSED` với exit code 0:
- BADGE-042: trước retry `Pending=1, Completed=0, Artifacts=0`; sau retry `Pending=0, Completed=1, Artifacts=1, PublishAttempts=1`.
- BADGE-077: trước retry `Pending=1, Completed=0, Artifacts=1, PublishAttempts=1`; sau retry `Pending=0, Completed=1, Artifacts=1, PublishAttempts=2`.
- Không có job `InFlight` bị treo sau exception; không có artifact thứ hai cho cùng badge.

Không sửa `Program.cs` hoặc simulator để làm xanh test. `verify.ps1` kiểm tra **starter người học chỉnh**, không dùng reference solution.
