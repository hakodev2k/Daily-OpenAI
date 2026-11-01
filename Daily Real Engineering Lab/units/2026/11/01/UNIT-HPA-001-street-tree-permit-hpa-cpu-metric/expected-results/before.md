# Before

- `pwsh -File ./scripts/reproduce.ps1` in `METRIC_UNAVAILABLE: container=audit-agent` và `REPRODUCE_PASS`.
- `pwsh -File ./scripts/verify.ps1` thất bại với `VERIFY_FAIL`.
- HPA snapshot có CPU usage nhưng `<unknown>/70%`.
- Mô hình chạy local, không gọi cluster.
