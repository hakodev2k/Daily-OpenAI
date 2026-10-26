# Before — starter nguyên bản

Chạy `./reproduce.ps1` trên starter chưa sửa. Kỳ vọng:

```text
intake.accepted=2 control.validated=2
dispatch.failed id=F-101 type=ObjectDisposedException
dispatch.failed id=F-102 type=ObjectDisposedException
summary accepted=2 delivered=0 failed=2 controlValidated=2
PASS: Original failure reproduced. This does NOT mean the system is fixed.
```

Đây là **expected output dựa trên static review**, chưa phải log chạy thực tế. Nếu không reproduce, kiểm tra SDK, working directory, và trạng thái starter đã bị sửa hay chưa.
