# UNIT-RL-001 — Hai tenant cùng nhận 429 dù quota còn trống

## Mục tiêu
Điều tra cách ASP.NET Core phân chia quota theo request context; phân biệt lỗi cấu hình ngưỡng, lỗi xác thực và lỗi cô lập quota. Tái hiện, thu thập evidence, đề xuất giả thuyết, sửa policy và kiểm chứng cả tình huống chặn đúng lẫn chặn sai.

## Bối cảnh thực tế
Nền tảng SaaS tạo tài liệu PDF cho nhiều doanh nghiệp. Mỗi tenant được phép 2 yêu cầu trong cửa sổ 5 phút trên **toàn bộ API**. Sau một lần tối ưu rate limiting, tenant Beta nhận 429 ngay ở yêu cầu đầu tiên dù Alpha vừa sử dụng API; ngược lại, Alpha có thể gọi thêm một endpoint khác dù đã dùng hết quota. Log xác thực vẫn thể hiện đúng tenant.

Lab dùng `PartitionedRateLimiter<HttpContext>` thực tế và một harness mô phỏng các request đã xác thực; không cần server, Docker, Azure hay NuGet bên ngoài .NET shared framework.

## Bạn cần làm gì
1. Chạy starter và lưu bảng `expected/actual` của bốn scenario A–D.
2. Đề xuất ít nhất ba hypothesis, dùng control D để loại trừ những giả thuyết yếu.
3. Chỉ sửa `starter/TenantQuotaPolicy.cs`. Không thay đổi `harness/` hoặc các expected assertions.
4. Kiểm tra tính độc lập giữa tenant, quota xuyên endpoint và hành vi anonymous.
5. Chạy verify rồi mới mở reference solution. Nêu giới hạn của mô phỏng khi scale-out nhiều instance.

## Yêu cầu môi trường
.NET 8 SDK (major 8, phiên bản 8.0.100 trở lên), PowerShell 7 hoặc Windows PowerShell 5.1. Không có NuGet package ngoài `Microsoft.AspNetCore.App`.

## Chạy nhanh
Từ thư mục unit:

```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
.\scripts\reproduce.ps1
```

Trên starter gốc, `EXPECTED_INCIDENT_REPRODUCED` và exit code 0 xác nhận đã **tái hiện triệu chứng**, không phải đã sửa. Lệnh `verify.ps1` phải thất bại trên starter và thành công sau khi bạn sửa policy.

## Những gì cần quan sát
- Scenario A: cùng endpoint, hai tenant độc lập có cùng nhận đủ ngân sách?
- Scenario B: cùng tenant, đổi endpoint có thay đổi kết quả kiểm soát quota?
- Scenario C: request không có tenant claim được phân loại nhất quán thế nào?
- Scenario D: một tenant gọi cùng endpoint ba lần có bị từ chối đúng ngưỡng?
- Vì sao một test chỉ dùng một endpoint và một tenant có thể báo xanh?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis tại [workspace](workspace/my-investigation.md).
3. Thử fix trên starter.
4. Verify cả regression và isolation.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [Phân tích và code tham chiếu](solution/README.md).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
45–60 phút (L2 Investigation, D4).
