# Expected results

## Before (original starter)
- `./reproduce.ps1` exit 0, in ra `REPRODUCED`.
- xUnit test suite exit 0 (green), dù `AcceptanceProbe` ghi `expiredDenied=False`.
- `./verify.ps1` không được thành công: acceptance contract không đạt; nếu chỉ sửa policy mà không sửa test, mutation gate vẫn không đạt.

## After (learner fixes starter)
- `dotnet test` exit 0 trong điều kiện bình thường.
- `AcceptanceProbe verify` ghi `expiredDenied=True; revokedDenied=True; validDenied=False`.
- Với `LAB_FAULT_INJECTION=accept-expired`, xUnit test suite phải exit khác 0 vì test phát hiện regression.
- `./verify.ps1` exit 0 sau khi xác nhận cả ba bước.

Không cần khẳng định số mili-giây cố định. Nếu restore thất bại, kiểm tra SDK và NuGet connectivity; nếu không reproduce, kiểm tra starter đã bị sửa hay chưa.
