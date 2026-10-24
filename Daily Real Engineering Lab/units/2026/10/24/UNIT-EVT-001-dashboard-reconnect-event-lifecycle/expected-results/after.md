# After — fix đúng
`./scripts/verify.ps1` exit 0, cả năm case PASS và `behavioral-mismatches=0`. Reconnect không làm tăng số lần xử lý; offline và disposed không nhận notification; happy path và repeat-connect không hồi quy. Kiểm tra deterministic, không phụ thuộc timing.
