# UNIT-IIS-001 — Reverse Proxy Path Base

## Mục tiêu
Điều tra một lỗi routing chỉ xuất hiện khi ASP.NET Core được publish sau IIS/reverse proxy dưới một subpath.

## Bối cảnh thực tế
Employee Benefits portal hoạt động đúng khi chạy trực tiếp ở root. Production expose ứng dụng dưới `/benefits`. Trang đầu tải được, nhưng một số navigation/redirect đưa người dùng ra ngoài vùng ứng dụng và trả 404.

## Bạn cần làm gì
Reproduce bằng simulator local, ghi evidence và hypothesis, sửa code trong `starter/`, sau đó chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7 hoặc Windows PowerShell

Không cần IIS thật; starter mô phỏng boundary mà reverse proxy tạo ra để lab deterministic.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script build và chạy starter với external mount path giống production, rồi kiểm tra URL được application sinh ra.

## Những gì cần quan sát
- URL public mà client dùng để vào ứng dụng.
- Path mà upstream application nhìn thấy.
- URL navigation được application trả về.
- URL đó còn nằm trong public application boundary hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
**Before:** request public vào đúng portal nhưng generated navigation không còn nằm trong public mount boundary.

**After:** generated navigation giữ đúng public application boundary và route nội bộ vẫn đúng.

Nếu không reproduce được, chạy `dotnet --info`, xác nhận .NET 8 SDK có sẵn rồi chạy `./run.ps1` để xem raw output.

## Estimated Time
45 phút