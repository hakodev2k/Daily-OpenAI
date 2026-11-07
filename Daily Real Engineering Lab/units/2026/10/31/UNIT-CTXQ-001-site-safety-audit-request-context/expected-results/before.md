# Before — starter gốc

Chạy `./scripts/reproduce.ps1`. Kỳ vọng:
- Hai dòng `ACCEPTED` cho TENANT-A và TENANT-B.
- `INGRESS: accepted=2; pending=2; both request contexts ended`.
- `REPRODUCED: deferred worker failed AUDIT_TENANT_CONTEXT_MISSING`.
- Exit code **0** nghĩa là lỗi đã được reproduce, không phải hệ thống đạt chuẩn.

`./scripts/verify.ps1` trên starter gốc phải exit khác 0: worker không hoàn thành audit khi request context đã kết thúc.
