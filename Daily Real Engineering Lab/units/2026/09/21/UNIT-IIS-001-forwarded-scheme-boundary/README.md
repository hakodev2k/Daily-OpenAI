# UNIT-IIS-001 — Forwarded Scheme Boundary

## Mục tiêu
Điều tra một redirect loop chỉ xuất hiện khi ASP.NET Core application chạy sau IIS/reverse proxy có TLS termination.

## Bối cảnh thực tế
Enterprise portal nhận HTTPS từ browser. Proxy terminate TLS rồi chuyển request nội bộ tới application bằng HTTP. Sau deployment, health check trực tiếp vẫn ổn nhưng browser gặp nhiều redirect liên tiếp.

## Bạn cần làm gì
Chạy starter và reproduction, quan sát scheme mà application nhìn thấy, xác định boundary giữa proxy và application, rồi sửa starter để request HTTPS gốc không bị redirect sai. Không tắt HTTPS policy chỉ để triệu chứng biến mất.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+

## Chạy nhanh
Chạy `./run.ps1`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Lab mô phỏng request đã đi qua TLS-terminating proxy và giữ evidence của forwarding metadata.

## Những gì cần quan sát
- Scheme application sử dụng trước redirect policy.
- Header mô tả scheme phía client/proxy.
- Thứ tự xử lý proxy metadata và redirect policy.

## Quy tắc làm lab
Chỉ sửa `starter/`. Không xóa HTTPS requirement, không hard-code URL production và không tin mọi forwarded header vô điều kiện.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ mở sau khi tự điều tra](solution/README.md)

## Expected Results
Sau khi sửa, `./verify.ps1` phải báo PASS: request được proxy xác nhận là HTTPS không bị redirect lại, còn request HTTP thật vẫn yêu cầu HTTPS.

## Estimated Time
50 phút.