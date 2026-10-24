# UNIT-CS-017 — Dashboard công việc hiển thị 0 dù sự kiện đã xử lý

## Mục tiêu
Reproduce sự không nhất quán giữa nhật ký xử lý và số liệu tổng hợp theo kỹ thuật viên; điều tra đường đi dữ liệu, sửa logic cập nhật mà vẫn giữ contract đọc snapshot và kiểm tra regression.

## Bối cảnh thực tế
Một dịch vụ .NET nhận các sự kiện hoàn thành/thất bại của công việc bảo trì hiện trường. Log xác nhận các sự kiện đã được nhận, nhưng dashboard cuối ngày vẫn hiển thị 0 cho một số kỹ thuật viên. Hệ thống demo không cần database hoặc Azure; dữ liệu mô phỏng cố định để kiểm tra lại kết quả.

## Bạn cần làm gì
1. Chạy starter, ghi lại log và bảng tổng hợp của ít nhất hai kỹ thuật viên.
2. Chạy reproduce, xác nhận triệu chứng, ghi ít nhất hai hypothesis trước khi xem hints.
3. Điều tra `starter/DailyRollup.cs`, đối chiếu luồng ghi với luồng đọc và contract của `WorkSummary`.
4. **Chỉ sửa `starter/DailyRollup.cs`**. Giữ nguyên `Program.cs`, `WorkSummary.cs`, scripts và reference.
5. Chạy verify để kiểm tra totals, tách biệt kỹ thuật viên, dữ liệu chưa tồn tại, input lỗi và snapshot không bị thay đổi ngoài ý muốn.

## Yêu cầu môi trường
.NET 8 SDK và PowerShell (Windows PowerShell 5.1 hoặc PowerShell 7+). Không cần NuGet package bên ngoài, Docker hoặc dịch vụ trả phí.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```
Hoặc: `dotnet run --project starter/FieldOpsLab.Starter.csproj -- run`.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 nghĩa là **triệu chứng gốc đã được reproduce**, không phải đã sửa xong. Sau khi tự sửa:
```powershell
./verify.ps1
```
Verify luôn chạy mã trong `starter/` do bạn chỉnh sửa, không chạy reference solution.

## Những gì cần quan sát
- Các sự kiện nào đã được ghi nhận ở input?
- Tổng `Completed`, `Failed`, `Total` trên dashboard có tương ứng với sự kiện không?
- Đọc lại cùng một kỹ thuật viên sau nhiều lần ghi có thay đổi không?
- Sau fix, snapshot đã đọc trước đó có thể vô tình thay đổi state nội bộ không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: chỉ mở [Reference Solution](solution/README.md) sau khi tự điều tra và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Ghi evidence trong [workspace](workspace/my-investigation.md).

## Estimated Time
35–50 phút.
