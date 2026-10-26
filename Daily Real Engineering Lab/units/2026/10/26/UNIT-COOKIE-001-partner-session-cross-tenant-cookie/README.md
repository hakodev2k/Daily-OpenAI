# UNIT-COOKIE-001 — Session của tenant A xuất hiện trong request gửi cho tenant B

## Mục tiêu

Điều tra một production incident trong adapter HTTP dùng chung cho nhiều tenant; tái hiện, thu thập bằng chứng request/session và sửa ranh giới cô lập mà không làm mất khả năng duy trì session hợp lệ.

## Bối cảnh thực tế

Một nền tảng procurement kết nối partner báo giá. Tenant Alpha đã đăng nhập partner, sau đó tenant Beta yêu cầu báo giá trên cùng một instance adapter. Log cho thấy Beta bị từ chối 403 thay vì 401 (chưa có session), và request của Beta chứa dấu hiệu session từ một tenant khác. Khi Beta đăng nhập, Alpha đôi khi mất quyền truy cập. Incident xuất hiện khi worker xử lý nhiều tenant liên tiếp, không phụ thuộc dữ liệu giá.

Bài lab chạy fake partner server trên loopback, dùng tenant và cookie **synthetic**, không có tài khoản hay secret thật.

## Bạn cần làm gì

1. Chạy starter để tái hiện chuỗi Alpha → Beta.
2. Thu thập status code và diagnostic marker do fake partner phát ra; không log raw cookie trong ứng dụng thật.
3. Ghi ít nhất ba giả thuyết (authorization phía server, routing, trạng thái client).
4. Sửa **chỉ** `starter/TenantGateway.cs` để Alpha và Beta có session độc lập; vẫn giữ connection/session reuse trong phạm vi tenant.
5. Chạy verification: Beta chưa đăng nhập phải là 401; sau đăng nhập Beta là 200; Alpha vẫn 200.

## Yêu cầu môi trường

- .NET SDK 8.x trở lên có khả năng build target `net8.0` và ASP.NET Core 8 runtime.
- PowerShell hoặc terminal chạy được `dotnet`.
- Không cần Docker, NuGet package ngoài framework, cloud, partner credentials hoặc port cố định.

## Chạy nhanh

Từ thư mục unit:

```powershell
pwsh -File ./setup.ps1
pwsh -File ./reproduce.ps1
```

Hoặc Bash:

```bash
dotnet restore starter/PartnerCookieLab.csproj
dotnet run --project starter/PartnerCookieLab.csproj -- reproduce
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter hiện tại, khởi động local fake partner trên một loopback port được OS cấp phát, gửi tuần tự các request và trả exit code 0 **chỉ khi** triệu chứng được tái hiện. Sau khi sửa code trong `starter/`, chạy `pwsh -File ./verify.ps1` (hoặc `dotnet run --project starter/PartnerCookieLab.csproj -- verify`).

## Những gì cần quan sát

- Status của Alpha sau login và Beta trước login.
- Diagnostic `X-Lab-Observed-Session` (chỉ là tên tenant giả lập, không phải token thật).
- Hành vi Alpha sau khi Beta đăng nhập.
- Tại sao lỗi chỉ xuất hiện khi một process xử lý nhiều tenant, trong khi test một tenant vẫn xanh.
- Bằng chứng nào loại trừ giả thuyết sai đường dẫn hay server luôn trả 403.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution

**Spoiler:** Chỉ đọc [Reference Solution](solution/README.md) và source `solution/TenantGateway.cs` sau khi tự sửa.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). Ghi chép tại [workspace/my-investigation.md](workspace/my-investigation.md).

## Estimated Time

65 phút. Xem thêm [evidence](evidence/incident-timeline.md) và [references](docs/references.md).
