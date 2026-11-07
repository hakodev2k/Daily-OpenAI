# UNIT-SEC-001 — Forwarded scheme trust behind a reverse proxy

## Mục tiêu
Điều tra một lỗi security boundary trong ASP.NET Core khi ứng dụng chạy sau reverse proxy và quyết định nguồn metadata nào được phép ảnh hưởng đến request scheme.

## Bối cảnh thực tế
Một internal account API được deploy sau ingress. Redirect URL và cookie policy hoạt động đúng qua ingress, nhưng QA phát hiện khi gọi trực tiếp service port thì client có thể làm ứng dụng tin rằng request là HTTPS dù transport thực tế là HTTP.

## Bạn cần làm gì
Reproduce hành vi, ghi hypothesis, xác định trust boundary, sửa `starter/` để chỉ proxy đã biết mới được phép cung cấp forwarded scheme, rồi chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script gửi hai request trực tiếp tới service: request bình thường và request có metadata thường do reverse proxy thêm vào.

## Những gì cần quan sát
- So sánh `scheme` mà application báo cáo giữa hai request.
- Xác định liệu một direct client có thể thay đổi security-relevant request metadata hay không.
- Ghi lại boundary nào đáng lẽ phải chịu trách nhiệm cho metadata đó.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints.md` theo thứ tự.

## Reference Solution
`solution/README.md` — spoiler, chỉ xem sau khi đã thử fix.

## Expected Results
Trước fix, direct client có thể tác động đến scheme quan sát bởi application. Sau fix, metadata chỉ có hiệu lực khi request đi qua trust boundary được cấu hình.

## Estimated Time
35–50 phút.