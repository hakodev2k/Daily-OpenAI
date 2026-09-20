# UNIT-IIS-001 — Reverse Proxy Prefix and PathBase

## Mục tiêu
Điều tra một lỗi routing/link-generation chỉ xuất hiện khi ASP.NET Core được publish sau IIS reverse proxy với một URL prefix.

## Bối cảnh thực tế
Employee Portal chạy ổn khi truy cập trực tiếp vào Kestrel. Qua gateway IIS, trang HTML vẫn mở được ở `/staff`, nhưng một số link do ứng dụng sinh ra trỏ sang `/profile` thay vì URL public mong đợi. Người dùng bấm link và nhận 404 ở proxy.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi evidence và ít nhất 2 hypothesis vào workspace.
3. Sửa code trong `starter/` để ứng dụng hiểu đúng external request path contract.
4. Chạy `verify.ps1`.
5. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7 hoặc Windows PowerShell
- Không cần IIS thật; starter mô phỏng boundary của reverse proxy bằng request headers.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` build và chạy test harness. Harness gửi request giống request đã đi qua proxy và kiểm tra URL ứng dụng sinh ra.

## Những gì cần quan sát
- External URL mà client dùng trước proxy.
- Path mà ASP.NET Core nhìn thấy sau proxy.
- Header mô tả prefix.
- URL được LinkGenerator sinh ra.
- Điểm nào trong pipeline biến đổi request trước routing/link generation.

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
[Spoiler — chỉ xem sau khi đã tự điều tra](solution/README.md)

## Expected Results
Before: generated profile URL thiếu public prefix và harness báo mismatch.

After: generated URL giữ đúng public prefix, routing nội bộ vẫn hoạt động, verify pass.

Nếu không reproduce được, chạy `dotnet --info`, xác nhận .NET 8 SDK tồn tại rồi chạy `dotnet run --project starter -- reproduce` trực tiếp.

## Estimated Time
45 phút.