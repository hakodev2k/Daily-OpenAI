# After — learner sửa thành công

Khi chạy `verify.ps1`, probe kỳ vọng:

```text
CRASH_INJECTED=True
HOLD_EXISTS_AFTER_RESTART=True
FINITE_TTL_AFTER_RESTART=True
RETRY_AFTER_3_MINUTES=True
NORMAL_CONTRACT_OK=True
VERIFICATION_PASSED
```

Không cần sleep thật: `ManualClock.Advance` điều khiển thời gian mô phỏng. Kiểm chứng vừa bao phủ crash path vừa giữ bất biến normal path: không cấp hai lượt giữ chỗ đồng thời, không ghi đè chủ sở hữu, cho phép đặt lại sau expiry.

Nếu probe thất bại, chạy `dotnet run --project .\starter\LeaseLab.csproj -- --probe` để xem các cờ chi tiết.
