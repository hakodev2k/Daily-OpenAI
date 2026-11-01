# Trước khi sửa
- `./scripts/run.ps1`: in `STABLE=8`, sau đó báo `LockRecursionException` và exit khác 0.
- `./scripts/reproduce.ps1`: in `REPRODUCED: LockRecursionException`, exit 0 vì kiểm tra được triệu chứng.
- `./scripts/verify.ps1`: thất bại ở nhánh mã mới; không được xem là PASS.
- Nếu không thấy lỗi, đảm bảo chưa thay đổi `starter/ThresholdRegistry.cs`; kiểm tra phiên bản .NET SDK.
