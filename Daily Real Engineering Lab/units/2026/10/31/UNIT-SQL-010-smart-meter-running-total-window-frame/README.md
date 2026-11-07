# UNIT-SQL-010 — Smart meter: Running total nhảy trước thời điểm ghi nhận

## Mục tiêu
Điều tra running total của SQL theo từng bản ghi, phân biệt thứ tự hiển thị với quy tắc tính toán và chứng minh bản sửa bằng regression contract.

## Bối cảnh thực tế
Dịch vụ đối soát điện năng của một quận nhận nhiều readings trong cùng một giây. Báo cáo cộng dồn phải phản ánh **từng reading** theo thứ tự ổn định. Một bản nâng cấp truy vấn khiến số cộng dồn ở reading đầu tiên của cùng timestamp nhảy lên quá sớm, dù tổng cuối ngày vẫn đúng.

## Bạn cần làm gì
1. Chạy dữ liệu mô phỏng và ghi lại reading nào sai.
2. Đưa ra ít nhất hai giả thuyết, kiểm tra SQL thay vì sửa dữ liệu.
3. Chỉnh `starter/Program.cs` để mọi reading có running total đúng và thứ tự ổn định.
4. Chạy `verify.ps1` trước khi xem reference solution.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK/runtime; NuGet internet cho lần restore đầu. Không cần database server, Docker hay cloud.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet restore .\starter\SmartMeterLedger.csproj
.\run.ps1
.\reproduce.ps1
```
Nếu chính sách máy chặn script, dùng `pwsh -NoProfile -File .\reproduce.ps1` hoặc `powershell -ExecutionPolicy Bypass -File .\reproduce.ps1` khi được phép.

## Cách reproduce vấn đề
`reproduce.ps1` chạy **starter** và trả exit code 0 chỉ khi phát hiện đúng signature lỗi ban đầu. Dữ liệu có 4 readings, hai readings cùng `recorded_at`. Ghi lại cột `CumulativeWh`, không chỉ tổng cuối.

## Những gì cần quan sát
So sánh từng hàng với business contract: reading 1 = 100 Wh, reading 2 = 120 Wh, reading 3 = 150 Wh, reading 4 = 190 Wh. Quan sát liệu thứ tự sắp xếp kết quả có bảo đảm cách tính cộng dồn hay không. Ghi evidence và giả thuyết trước khi sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [solution/README.md](solution/README.md) sau khi đã thử sửa và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` chạy trên **starter do bạn sửa**, không chạy trên solution.

## Estimated Time
35–45 phút (L1 / D3). Lưu ghi chú vào [workspace/my-investigation.md](workspace/my-investigation.md).
