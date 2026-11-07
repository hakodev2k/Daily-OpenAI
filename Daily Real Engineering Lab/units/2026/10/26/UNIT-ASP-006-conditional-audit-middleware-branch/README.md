# UNIT-ASP-006 — Conditional Audit Middleware Breaks Maintenance API Routes

## Mục tiêu
Điều tra ASP.NET Core middleware pipeline bằng bằng chứng request/response; khôi phục API mà vẫn giữ audit policy đúng phạm vi.

## Bối cảnh thực tế
Một hệ thống bảo trì thiết bị nhà máy vừa bật audit policy có điều kiện. Health check vẫn xanh, nhưng maintenance UI báo lỗi. Team cần root-cause analysis trước khi rollback.

## Bạn cần làm gì
Reproduce; ghi status/body/header của bốn request; viết ít nhất hai hypotheses; sửa **starter/Program.cs**; verify correctness và audit boundaries. Acceptance contract: [scenario](docs/scenario.md).

## Yêu cầu môi trường
.NET SDK 8+ có khả năng build `net8.0`, PowerShell (khuyến nghị). Không cần Docker, NuGet ngoài shared framework, cloud hay cổng HTTP.

## Chạy nhanh
Từ thư mục unit:
```powershell
pwsh -File ./scripts/setup.ps1
pwsh -File ./scripts/reproduce.ps1
pwsh -File ./scripts/run.ps1
```
Windows PowerShell có thể dùng `powershell -File`. Không có PowerShell: `dotnet run --project ./starter/PipelineLab.csproj -- reproduce`.

## Cách reproduce vấn đề
Script `reproduce.ps1` chạy starter nguyên bản và exit 0 **khi phát hiện đúng triệu chứng**. Sau khi sửa, reproduce có thể exit 1; dùng verify để kiểm tra fix.

## Những gì cần quan sát
So sánh `/health`, `/api/v1/maintenance/active`, `/api/v1/maintenance/history`, `/missing`. Ghi status, response body, `X-Audit-Policy`, sau đó phân biệt “đã đi qua audit” với “đã tới business handler”. Ghi hypothesis vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Verify bằng `pwsh -File ./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
35–45 phút. Validation mode: **static-review** (không tuyên bố đã chạy .NET CLI).
