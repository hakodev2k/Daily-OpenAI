# UNIT-PIPEX-001 — Telemetry băng chuyền lặp đọc frame chưa hoàn chỉnh

## Mục tiêu
Tự reproduce triệu chứng, thu thập bằng chứng về vòng đọc streaming, xác định nguyên nhân và sửa mà không làm mất dữ liệu.

## Bối cảnh thực tế
Gateway băng chuyền nhận telemetry UTF-8, mỗi bản tin kết thúc bằng newline. Một bản tin có thể đến qua nhiều TCP fragments. Trong ca trực, gateway liên tục ghi nhận cùng một fragment dù thiết bị chưa gửi byte mới.

## Bạn cần làm gì
Chạy starter, ghi ít nhất hai hypothesis, quan sát số incomplete observations trước/sau khi có dữ liệu mới. Chỉ chỉnh `starter/FrameReader.cs`; giữ nguyên oracle trong `starter/Program.cs`.

## Yêu cầu môi trường
PowerShell 5.1+/7+, .NET SDK 8+ và ASP.NET Core shared framework 8. Không cần cloud, Docker hoặc database.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
./verify.ps1
```
`reproduce.ps1` xác nhận triệu chứng baseline; `verify.ps1` FAIL trên starter và PASS sau khi sửa.

## Cách reproduce vấn đề
```powershell
./run.ps1
dotnet run --project starter/ConveyorTelemetry.csproj -- reproduce
```

## Những gì cần quan sát
- Producer mới gửi fragment đầu, chưa có delimiter.
- Bao nhiêu lần reader quan sát dữ liệu chưa hoàn chỉnh khi không có write mới?
- Reader có đợi fragment tiếp theo không?
- Khi nhận fragment thứ hai, có trả đúng một frame đầy đủ không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference solution và trade-offs](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
35–45 phút.
