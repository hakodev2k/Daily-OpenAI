# Trước khi sửa

Khi chạy starter gốc bằng `scripts/reproduce.ps1`:
- `initial=Accepted`, provider v1.
- Sau rotation v2: provider=v2, control partner=Accepted, gateway=Unauthorized, sentVersion=v1.
- Sau rotation v3: provider=v3, control partner=Accepted, gateway=Unauthorized, sentVersion=v1.
- `oldVersionControl=Unauthorized`.
- `INCIDENT_REPRODUCED`, exit code 0.

`scripts/verify.ps1` phải trả `VERIFICATION_FAILED` và exit code 22 (PowerShell wrapper báo lỗi). Nếu không reproduce, xác nhận bạn đang chạy starter gốc, .NET 8 SDK và `Program.cs` chưa bị sửa.
