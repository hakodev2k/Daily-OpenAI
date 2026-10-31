# Before — starter nguyên bản

Khi chạy `reproduce.ps1`, probe kỳ vọng:

```text
CRASH_INJECTED=True
HOLD_EXISTS_AFTER_RESTART=True
FINITE_TTL_AFTER_RESTART=False
RETRY_AFTER_3_MINUTES=False
NORMAL_CONTRACT_OK=True
REPRODUCTION_CONFIRMED
```

Đây là dữ liệu mô phỏng deterministic, không phải log đo từ Redis production. Chạy `verify.ps1` trước khi sửa phải thất bại. Nếu không reproduce, kiểm tra .NET SDK 8.x và đảm bảo chưa thay đổi `starter/`.
