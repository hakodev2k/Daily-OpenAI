# Trước khi sửa

Với starter nguyên bản, `./scripts/reproduce.ps1` trả exit code 0 và quan sát:

```text
dispatchTaskCompleted=True
sentBeforeRelease=0
sentAfterRelease=3
REPRODUCED: premature completion
```

`./scripts/verify.ps1` trả exit code 1 với thông báo `FAIL: batch completion is not tied to all gateway sends`. Giá trị Boolean do .NET in có thể là `True`/`False`.

Nếu không reproduce được, xác nhận bạn đang chạy starter nguyên bản, đúng thư mục unit và SDK .NET 8; chạy lại `./scripts/setup.ps1`.
