# UNIT-ASPNET-002 — Webhook hợp lệ nhưng endpoint nhận body rỗng

## Mục tiêu
Điều tra một regression trong ASP.NET Core request pipeline khi middleware audit đọc raw request body nhưng endpoint phía sau không còn nhận được payload hợp lệ.

## Bối cảnh thực tế
Một payment webhook service vừa bổ sung middleware audit để lưu raw payload phục vụ đối soát. Sau deploy, đối tác vẫn gửi JSON hợp lệ nhưng endpoint bắt đầu trả lỗi parse/validation.

## Bạn cần làm gì
1. Chạy starter.
2. Reproduce lỗi.
3. Ghi ít nhất 2 hypotheses trước khi sửa.
4. Theo dõi request body qua middleware và endpoint.
5. Sửa `starter/` để audit vẫn đọc được payload và endpoint vẫn nhận đúng dữ liệu.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 5.1+ hoặc PowerShell 7+
- Port 5086 trống

## Chạy nhanh
```powershell
./run.ps1
```

Trong terminal khác:
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` khởi động starter, gửi một payment webhook JSON hợp lệ và kiểm tra rằng request chưa đạt contract mong muốn.

## Những gì cần quan sát
- Audit middleware có đọc được payload hay không.
- HTTP status của endpoint.
- Endpoint có bind được `paymentId` và `amount` hay không.
- Trạng thái/position của body stream trước và sau middleware.
- Có exception hay chỉ là validation failure.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
> Spoiler: chỉ xem sau khi đã tự thử.
- [Reference solution](solution/README.md)

## Expected Results
Before: audit đọc được payload nhưng endpoint không xử lý request thành công.
After: audit vẫn đọc được payload, endpoint trả `200 OK` và echo đúng dữ liệu webhook.

## Estimated Time
30–45 phút.
