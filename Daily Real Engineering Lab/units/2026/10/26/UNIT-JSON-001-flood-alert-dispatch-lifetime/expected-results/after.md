# After — learner starter đã sửa

Chạy `./verify.ps1`. Kỳ vọng:

```text
intake.accepted=2 control.validated=2
dispatch.delivered id=F-101 zone=Delta severity=high
dispatch.delivered id=F-102 zone=Harbor severity=critical
summary accepted=2 delivered=2 failed=0 controlValidated=2
PASS: Learner-edited starter delivers both alerts without processing failures.
```

Đây là **expected output**, không phải bằng chứng đã chạy trong môi trường tạo lab. Nếu `verify.ps1` fail, so sánh stdout và exit code, đảm bảo thay đổi nằm trong `starter/` và cả hai payload vẫn giữ đúng ID, zone, severity.
