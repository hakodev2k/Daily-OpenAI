# Before — starter chưa sửa
- `./scripts/setup.ps1` build thành công nếu có .NET SDK 8.
- `./scripts/reproduce.ps1` in `A still running when C entered: True` và `REPRODUCED`, exit 0.
- `./scripts/verify.ps1` in `CONTRACT FAILED`, exit 1.
- B bị hủy và callback B không chạy; fault-recovery guardrail vẫn pass.
- Nếu không tái hiện: bảo đảm chạy từ đúng unit, chưa sửa `starter/WorkGate.cs`, và dùng .NET 8; kiểm tra stderr của harness.
