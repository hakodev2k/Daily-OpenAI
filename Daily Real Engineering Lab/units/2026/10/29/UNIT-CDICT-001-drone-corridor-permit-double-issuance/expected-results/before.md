# Before — starter chưa sửa

```powershell
./reproduce.ps1
# OBSERVED issued=2; sameCachedPermit=True; reusedPermit=True; issuedAfterOtherKey=3
# EXPECTED_SYMPTOM: duplicate external issuance for one cache key
# exit code: 0

./verify.ps1
# OBSERVED issued=2; sameCachedPermit=True; reusedPermit=True; issuedAfterOtherKey=3
# VERIFY_FAIL InvalidOperationException: same-key concurrent requests must issue once
# exit code: 1
```

Harness dùng hai worker LongRunning và cổng đồng bộ ở fake authority. Nếu máy quá tải khiến hai callback không vào cổng trong 3 giây, chạy lại; ghi nhận đây là giới hạn của việc reproduce race bằng lịch chạy thread, không phải lỗi input.
