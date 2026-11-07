# Before — Baseline chưa sửa

Chạy `./reproduce.ps1` phải in `SLOW_1200_FLAGGED=False`, `SLOW_1900_FLAGGED=True`, `SLOW_2500_FLAGGED=False` và `REPRODUCTION_CONFIRMED`.

`./verify.ps1` phải **thất bại** trên `starter/LatencyPolicy.cs` ban đầu, vì ít nhất các hợp đồng duration trên một giây không đạt. `reproduce.ps1` là fixture cố định; sau khi learner sửa code, nó vẫn tái hiện baseline cũ.
