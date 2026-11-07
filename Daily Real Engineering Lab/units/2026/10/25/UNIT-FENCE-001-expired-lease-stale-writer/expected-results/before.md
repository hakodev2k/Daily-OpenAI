# Before — starter

- `scripts/reproduce.ps1` in `EXPECTED_INCIDENT_REPRODUCED` và exit 0.
- `scripts/run.ps1` hiển thị A epoch 1, B epoch 2, delayed A accepted=True, final plan=plan-from-A.
- `scripts/verify.ps1` thất bại ở kiểm tra stale owner; exit khác 0.
