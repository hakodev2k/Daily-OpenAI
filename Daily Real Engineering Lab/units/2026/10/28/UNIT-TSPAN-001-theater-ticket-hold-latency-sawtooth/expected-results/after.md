# After — Learner fix

Sau khi sửa duy nhất `starter/LatencyPolicy.cs`, `./verify.ps1` phải in `CHECKS_PASSED=17`, `VERIFIED: learner latency policy satisfies SLA contracts` và `VERIFICATION_CONFIRMED`.

Với budget 750 ms: 749/750 ms không vi phạm, 751/1000/1200/1900/2500 ms vi phạm. Với budget 1500 ms: 1200 và 1500 ms không vi phạm, 1501 ms vi phạm. Duration âm hoặc budget không dương bị từ chối; duration chỉ hơn ngưỡng 1 tick vẫn phải được tính chính xác.
