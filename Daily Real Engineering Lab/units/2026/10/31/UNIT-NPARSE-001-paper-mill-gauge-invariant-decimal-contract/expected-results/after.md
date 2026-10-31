# After — Fixed starter

- en-US, de-DE, fr-FR, vi-VN đều đọc `1.234m`, `0.875m`, `2.500m`.
- Token `1,234`, `1.2.3`, rỗng hoặc thiếu RollId bị từ chối bằng `FormatException`.
- `verify.ps1` chạy chính starter đã sửa: 9 PASS, 0 FAIL, exit 0.
- `reproduce.ps1` sau fix exit 1 (lỗi không còn).
