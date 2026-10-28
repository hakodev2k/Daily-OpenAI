# UNIT-AUTHZ-001 — Water Audit Portal: anonymous access after API expansion

## Mục tiêu
Điều tra một sự cố kiểm soát truy cập trong ASP.NET Core Minimal API, xác định bằng chứng ở HTTP boundary, đề xuất sửa đổi có regression tests và đánh giá ảnh hưởng khi thêm endpoint mới.

## Bối cảnh thực tế
Portal kiểm định chất lượng nước cung cấp báo cáo nội bộ cho nhân viên được cấp quyền và một health endpoint công khai. Sau đợt mở rộng API, hệ thống giám sát ghi nhận một số request không có danh tính vẫn đọc được dữ liệu kiểm định. Frontend vẫn yêu cầu đăng nhập, nhưng API có thể được gọi trực tiếp.

Đây là **mô phỏng bảo mật local**, sử dụng dữ liệu giả và authentication handler giả lập. Không triển khai handler này lên production.

## Bạn cần làm gì
1. Chạy starter và ghi lại HTTP status của bốn trường hợp.
2. Phân biệt authentication, authorization và endpoint metadata bằng evidence.
3. Viết hypothesis trước khi thay đổi `starter/Program.cs`.
4. Sửa trên **starter/**, giữ public health route, ngăn anonymous read ở cả hai resource API.
5. Chạy verification; xem reference solution sau khi tự hoàn thành.

## Yêu cầu môi trường
- .NET SDK 8.x hoặc SDK mới hơn có khả năng build `net8.0` và .NET 8 runtime.
- PowerShell 7+ (hoặc chạy lệnh `dotnet` tương đương).
- Không cần Azure, database, Docker hay NuGet package bên ngoài.

## Chạy nhanh
Tại thư mục unit:

```powershell
./reproduce.ps1
./verify.ps1
```

Lệnh tương đương không cần PowerShell:

```bash
dotnet run --project starter/AuthBoundary.csproj -- --reproduce
dotnet run --project starter/AuthBoundary.csproj -- --verify
```

`reproduce` kiểm tra **starter gốc** có thể tái hiện triệu chứng; `verify` kiểm tra **code bạn đã sửa trong starter/**. Trước khi sửa, `verify` được kỳ vọng trả exit code 1.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trước khi sửa. Nếu muốn quan sát bằng HTTP client riêng, chạy `./run.ps1`, lấy loopback URL in ra và gọi `/api/audits/A-204`, `/api/audits/export`, `/health` với và không có header `X-Demo-User: auditor`.

## Những gì cần quan sát
Ghi status code, endpoint, có/không có header và kết quả `reproduce`. So sánh với [incident evidence](evidence/incident-log.md), đánh dấu phần nào là dữ liệu quan sát và phần nào chỉ là hypothesis. Không suy luận rằng frontend login đồng nghĩa API đã được bảo vệ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify cả hành vi bị từ chối và hành vi hợp lệ.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi đã thử tự sửa:** [Reference Solution](solution/README.md).

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
70 phút. Không cần tốc độ xử lý hay port cố định; test tự cấp loopback port.
