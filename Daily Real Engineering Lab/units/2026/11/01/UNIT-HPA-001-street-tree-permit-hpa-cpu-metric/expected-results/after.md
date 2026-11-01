# After

- `pwsh -File ./scripts/verify.ps1` in `METRIC_READY`, `DESIRED_REPLICAS=3`, `VERIFY_PASS`.
- Với cấu hình tham khảo: CPU usage 510m; CPU requests 600m; utilization 85%; ceil(2×85/70) = 3.
- `reproduce.ps1` sau fix có thể FAIL vì không còn triệu chứng ban đầu.
- Trên cluster thật còn phải xét tolerance, readiness, stabilization, scheduling và downstream capacity.
