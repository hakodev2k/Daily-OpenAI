# UNIT-GETORADD-001 — Thẻ tham quan được cấp hai lần khi hai quầy check-in đồng thời

## Mục tiêu
Điều tra sự khác nhau giữa một bản ghi cache cuối cùng và số lần hệ thống ngoài thực hiện side effect dưới concurrency.

## Bối cảnh thực tế
Hai kiosk đồng thời cấp thẻ năm cho cùng một hội viên. Cache chỉ chứa một thẻ nhưng ledger ghi nhận nhiều lần cấp. Việc cấp thẻ ngoài đời có phí và không thể chỉ xóa bản ghi dư.

## Bạn cần làm gì
Chạy baseline, thu thập issued count và returned serial, đưa ra ít nhất hai hypothesis, xác định invariant, sửa `starter/Program.cs` và chạy regression.

## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần NuGet package ngoài hoặc dịch vụ cloud.

## Chạy nhanh
```powershell
dotnet restore ./starter/PassLab.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` (hoặc `dotnet run --project ./starter/PassLab.csproj -c Release`). Hai task được mở cùng lúc; so sánh `issued` với `cache`.

## Những gì cần quan sát
Quan sát số lần ledger cấp thẻ, số phần tử cache và serial được hai kiosk nhận. Một phần tử cache có đồng nghĩa một lần cấp thật không? Thử lặp lại và ghi lại kết quả.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[01](hints/hint-01.md) · [02](hints/hint-02.md) · [03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Trước sửa: ledger có thể cấp nhiều hơn một thẻ cho cùng hội viên dù cache có một entry. Sau sửa: 10 lần replay đều `issued=1`, serial giống nhau và cache có một entry.

## Estimated Time
45 phút.
