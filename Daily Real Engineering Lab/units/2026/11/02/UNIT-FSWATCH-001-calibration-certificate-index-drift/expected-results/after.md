# After

verify.ps1 trả 0, chương trình xuất LAB_RESULT=PASS.

- Initial: A valid:v1, B valid:v1, C review:v1.
- Phase 1: A suspended:v2, B valid:v2, D valid:v1; C không còn.
- Tick lặp lại giữ nguyên phase 1.
- Phase 2: A suspended:v2, D expired:v2, E valid:v1; B/C không còn.
- scratch.tmp không xuất hiện; notificationsReceived=5, maintenanceTicks=3.

Không cần đo timing vì harness xác định.
