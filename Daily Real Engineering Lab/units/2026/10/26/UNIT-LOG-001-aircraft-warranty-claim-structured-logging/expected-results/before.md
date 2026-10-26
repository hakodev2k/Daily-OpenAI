# Before
- `./reproduce.ps1` exit 0 và in `REPRODUCED`.
- Nội dung hiển thị chứa `WC-4102`, `AV-29B`, `APAC`.
- `Indexed attributes` thiếu `ClaimId`, `PartNumber`, `Region`.
- `./verify.ps1` exit khác 0, in `VERIFY FAIL`.
- Nếu build không được, kiểm tra `dotnet --info` và targeting pack net8.0.
