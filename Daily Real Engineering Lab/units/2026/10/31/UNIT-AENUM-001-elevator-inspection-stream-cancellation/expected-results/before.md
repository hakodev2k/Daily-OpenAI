# Before

`./scripts/reproduce.ps1` (exit 0) tái hiện:

```text
RESULT=FAIL; cancellationObserved=False; received=1,2,3,4
```

`./scripts/verify.ps1` exit 1 khi starter chưa sửa. Warning CS8425 có thể xuất hiện khi build.
