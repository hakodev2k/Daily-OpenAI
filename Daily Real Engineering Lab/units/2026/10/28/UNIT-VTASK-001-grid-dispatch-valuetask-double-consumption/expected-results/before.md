# Before — starter nguyên bản

```text
GATEWAY_REQUESTS=1
AUDIT_EVENTS=0
OBSERVED_EXCEPTION=InvalidOperationException
REPRODUCED: valid signal failed before receipt
REPRODUCTION_CONFIRMED
```

Đây là failure được kiểm soát: `reproduce.ps1` trả thành công khi xác nhận đúng triệu chứng. `verify.ps1` trên starter phải thất bại.
