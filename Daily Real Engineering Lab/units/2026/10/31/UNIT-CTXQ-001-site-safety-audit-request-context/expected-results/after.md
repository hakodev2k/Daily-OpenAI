# After — sửa starter

`./scripts/verify.ps1` phải exit **0** và in:
- `PASS: tenant attribution remains isolated after request completion`
- `PASS: invalid request rejected before enqueue`
- `PASS: no invalid work remains queued`
- `VERIFIED: all checks ran against learner-editable starter.`

Hai bản ghi theo thứ tự PERMIT-101/TENANT-A và PERMIT-202/TENANT-B; không có audit nào cho request thiếu tenant. Không được dùng reference solution để thay thế starter trong verify.
