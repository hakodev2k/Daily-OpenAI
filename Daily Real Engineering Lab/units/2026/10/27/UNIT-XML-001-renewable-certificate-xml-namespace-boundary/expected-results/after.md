# After — learner fix

```text
Batch=OCT-27; readingCount=2; totalKwh=20.0
PASS default-namespace payload: 2 readings, 20.0 kWh
PASS prefix-equivalent payload: 2 readings, 20.0 kWh
PASS unsupported contract version: rejected with InvalidDataException.
PASS missing kwh attribute: rejected with InvalidDataException.
VERIFY PASS
```

Cách hiển thị thập phân có thể khác nhau nhưng giá trị phải bằng 20m. `verify.ps1` trả exit code 0; `reproduce.ps1` không còn tái hiện lỗi và trả exit code 1.
