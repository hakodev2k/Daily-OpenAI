# Before — starter

```text
Batch=OCT-27; readingCount=0; totalKwh=0
REPRODUCED: accepted batch with zero readings.
```

`reproduce.ps1` trả exit code 0. `verify.ps1` trả exit code 1 và ghi nhận thất bại ở cả hai payload hợp lệ và các trường hợp invalid. Đây là trạng thái ban đầu có chủ đích, không phải verification thành công.
