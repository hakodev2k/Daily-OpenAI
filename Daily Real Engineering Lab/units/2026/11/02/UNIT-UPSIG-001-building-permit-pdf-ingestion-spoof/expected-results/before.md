# Before — baseline

- `pwsh ./scripts/reproduce.ps1` thoát mã 0 và in `REPRODUCED`.
- Hồ sơ synthetic không phù hợp được admission chấp nhận, nhưng downstream parser báo lỗi.
- `verify.ps1` trên starter ban đầu có ít nhất một `FAIL` và exit code 1.
- Không dùng con số latency làm điều kiện pass/fail.
