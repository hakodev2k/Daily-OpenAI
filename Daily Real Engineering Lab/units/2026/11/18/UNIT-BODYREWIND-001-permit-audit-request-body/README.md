# UNIT-BODYREWIND-001 — Hồ sơ cấp phép mất payload sau audit middleware
## Mục tiêu
Điều tra luồng request trong ASP.NET Core, tìm bằng chứng và khắc phục mà vẫn giữ audit.
## Bối cảnh thực tế
Sau khi thêm audit middleware, cổng cấp phép xây dựng trả HTTP 400 cho JSON hợp lệ. Khi bỏ audit, endpoint chấp nhận hồ sơ.
## Bạn cần làm gì
Reproduce, ghi ba hypothesis, quan sát audit/handler byte counts, sửa starter, verify.
## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần cloud, Docker hoặc NuGet bên ngoài.
## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```
## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`; ghi status và byte counts. Reproduce PASS nghĩa là lỗi gốc được tái hiện, không phải đã sửa.
## Những gì cần quan sát
Audit và endpoint có nhìn thấy cùng số byte không? Đối chiếu trạng thái HTTP; đừng đoán lỗi JSON trước khi xem evidence.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Sửa `starter/Program.cs`.
4. Verify.
5. Sau đó mới xem solution.
## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)
## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md)
## Expected Results
Trước: audit có payload, endpoint rỗng, status 400. Sau: cả hai đọc đủ JSON, status 202, verify PASS.
## Estimated Time
45 phút.
