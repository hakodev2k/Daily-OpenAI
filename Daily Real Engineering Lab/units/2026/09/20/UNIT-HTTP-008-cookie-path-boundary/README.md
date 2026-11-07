# UNIT-HTTP-008 — Cookie Path Boundary

## Mục tiêu
Điều tra một lỗi HTTP state khiến đăng nhập thành công nhưng một số route trong cùng ứng dụng lại hành xử như chưa đăng nhập.

## Bối cảnh thực tế
Merchant Admin Portal vừa tách một số endpoint thành các route mới. Người dùng đăng nhập ở `/admin/login`, vào `/admin/orders` bình thường, nhưng khi chuyển sang `/reports/daily` thì bị yêu cầu đăng nhập lại. Backend không ghi nhận session lookup cho request lỗi.

## Bạn cần làm gì
Chạy starter, reproduce chuỗi request, quan sát cookie jar và request headers, ghi hypothesis, sửa cấu hình trong `starter/Program.cs`, rồi chạy verify.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script chạy deterministic simulator cho login và hai navigation requests.

## Những gì cần quan sát
- Response login có phát hành cookie hay không.
- Request nào gửi cookie trở lại server.
- Route nào bị coi là unauthenticated.
- Không chỉ nhìn status code; so sánh request path với cookie metadata.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Before: login thành công; `/admin/orders` authenticated; `/reports/daily` unauthenticated.

After: cả hai protected routes nhận đúng authentication state mà không làm cookie rộng hơn phạm vi ứng dụng cần thiết.

Nếu không reproduce được, chạy `dotnet --version` và xác nhận SDK 8.x có sẵn.

## Estimated Time
35 phút.
