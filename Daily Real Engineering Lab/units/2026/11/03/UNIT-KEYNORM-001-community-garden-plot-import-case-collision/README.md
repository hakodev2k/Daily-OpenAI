# UNIT-KEYNORM-001 — Bản ghi lô vườn cộng đồng biến mất khi import

## Mục tiêu
Điều tra sai lệch giữa số dòng CSV hợp lệ và số lô vườn được lưu, xây dựng hợp đồng kiểm tra tính duy nhất có thể kiểm chứng.

## Bối cảnh thực tế
Hệ thống phân bổ lô vườn của quận nhận dữ liệu từ hai cổng đăng ký. Báo cáo nguồn có 4 đăng ký, nhưng sau import chỉ còn 3 lô; một hộ dân không tìm được đăng ký. Không có lỗi mạng hay database.

## Bạn cần làm gì
Chạy starter, ghi lại số dòng, số lô và chủ sở hữu bị mất; đưa ít nhất hai giả thuyết, sửa code trong `starter/Program.cs` để không mất dữ liệu âm thầm. Định nghĩa rõ chính sách khi mã lô trùng nhau theo cách so sánh của nghiệp vụ.

## Yêu cầu môi trường
.NET SDK 8.0; PowerShell 7 hoặc Windows PowerShell.

## Chạy nhanh
```powershell
dotnet run --project starter/GardenImport.csproj
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script phải báo phát hiện sai lệch trong starter gốc.

## Những gì cần quan sát
So sánh `InputRows`, `StoredPlots`, `Owners`; kiểm tra dữ liệu hai cổng, mã lô và hợp đồng so sánh định danh. Phân biệt mất dòng với lỗi ghi dữ liệu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md) — chỉ xem sau khi thử sửa.

## Expected Results
Trước: 4 dòng đầu vào, 3 lô được lưu, thiếu một chủ sở hữu. Sau: phát hiện xung đột và từ chối toàn bộ batch có mã lô trùng theo chính sách, không silently overwrite; batch không trùng được nhận đủ.

## Estimated Time
35–45 phút.
