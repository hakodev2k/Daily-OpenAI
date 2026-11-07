# UNIT-CLAIMS-001 — Certification Review: Valid Identity, Unexpected Access Denial

## Mục tiêu
Điều tra sự khác biệt giữa identity đã xác thực và quyết định authorization trong ASP.NET Core; thu thập bằng chứng, xác định contract giữa identity provider và policy, sửa lỗi mà không mở rộng quyền ngoài ý muốn.

## Bối cảnh thực tế
Portal kiểm định thiết bị công nghiệp sử dụng nhà cung cấp danh tính bên ngoài. Sau một đợt tích hợp, người có quyền `Reviewer` đăng nhập thành công nhưng bị từ chối khi mở màn hình duyệt chứng nhận. Người chỉ có quyền `Reader` vẫn bị từ chối như mong đợi. Helpdesk xác nhận token đã qua kiểm tra issuer, audience, signature và thời hạn.

Lab mô phỏng **bước tạo principal sau khi token đã được xác thực**; không triển khai hoặc giả lập quy trình xác thực chữ ký JWT.

## Bạn cần làm gì
1. Chạy mô phỏng và tái hiện sự khác biệt giữa danh sách role nhận được với kết quả policy.
2. Ghi ít nhất ba hypothesis, đối chiếu evidence và kiểm tra claim/identity/policy.
3. Chỉnh sửa **chỉ mã trong `starter/`** để người có `Reviewer` được phép truy cập.
4. Chạy regression checks để chứng minh `Reader`, role gần giống và principal chưa xác thực vẫn bị chặn.
5. Đề xuất cách áp dụng an toàn khi triển khai với JWT Bearer thật.

## Yêu cầu môi trường
- .NET SDK **8.0.x**, PowerShell 7+ hoặc Windows PowerShell.
- Không cần Docker, database, cloud, NuGet package ngoài shared framework `Microsoft.AspNetCore.App`.
- `global.json` pin SDK 8.0.100 và cho phép roll-forward trong cùng feature train/major 8.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Có thể chạy không dùng PowerShell:

```bash
dotnet run --project starter/RoleClaimLab.csproj
dotnet run --project checks/ContractChecks.csproj -- reproduce
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy **mã starter hiện tại**, exit code 0 khi hai trường hợp `Reviewer` hợp lệ bị từ chối và các negative guardrails vẫn hoạt động. Nếu đã sửa đúng, reproduce sẽ không còn pass — đây là hành vi mong đợi.

## Những gì cần quan sát
- Token đã xác thực và principal có role data nhưng policy có cho phép hay không?
- Các trường hợp `Reader`, thiếu role, role sai chữ hoa/thường, principal chưa xác thực có kết quả gì?
- So sánh danh sách claims, `IsAuthenticated`, kết quả policy và các thuộc tính identity liên quan.
- Dùng [incident log](evidence/incident-log.md) để phân biệt triệu chứng với giả thuyết.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Chạy `./scripts/verify.ps1` — script kiểm tra **mã bạn vừa sửa**, không chạy reference solution.
5. Chỉ sau đó mới mở solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler — chỉ đọc [solution/README.md](solution/README.md) sau khi tự reproduce, điều tra và thử sửa.

## Expected Results
- **Before:** reproduce exit 0, verify exit khác 0.
- **After:** verify exit 0; cả positive cases được phép và toàn bộ negative guardrails tiếp tục bị từ chối.
- Chi tiết: [before](expected-results/before.md), [after](expected-results/after.md).

## Estimated Time
50 phút. Bài mở rộng: [Senior challenge](extensions/senior-challenge.md).
