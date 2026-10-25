# UNIT-UNICODE-001 — Một vé tham quan được ghi nhận hai lần sau đồng bộ từ hai thiết bị

## Mục tiêu
Điều tra một lỗi nhận diện mã vé trong pipeline import C#; dùng bằng chứng thực nghiệm để xác lập hợp đồng so sánh, sửa lỗi và bảo vệ hành vi bằng regression check.

## Bối cảnh thực tế
Hệ thống cổng vào của một bảo tàng khu vực nhận sự kiện quét vé từ kiosk và thiết bị di động. Một số mã vé trông giống nhau trên dashboard nhưng được tính thành hai lượt vào, khiến báo cáo sức chứa sai. Các mã khác biệt thực sự phải vẫn được tính riêng.

## Bạn cần làm gì
1. Chạy starter và ghi lại EventId được chấp nhận.
2. Đưa ra ít nhất hai giả thuyết trước khi mở hints.
3. Thu thập bằng chứng về chuỗi đầu vào và quy tắc so sánh.
4. Chỉnh sửa **chỉ** `starter/BadgeImporter.cs` để bảo đảm một vé hợp lệ chỉ được chấp nhận một lần theo hợp đồng.
5. Chạy verify và giải thích tại sao không gộp nhầm mã khác nhau.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell 5.1; không cần database, Docker, Azure hay NuGet package ngoài.

## Chạy nhanh
Tại thư mục unit:
```powershell
dotnet --version
./run.ps1
./reproduce.ps1
```
Trên Windows PowerShell, nếu cần dùng `powershell -File .\reproduce.ps1`. Trên PowerShell 7, dùng `pwsh -File ./reproduce.ps1`.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 nghĩa là script **đã xác nhận triệu chứng trên starter**, không phải đã sửa lỗi. `./verify.ps1` phải thất bại trên starter gốc; sau khi sửa, chạy lại `./verify.ps1` để xác nhận.

## Những gì cần quan sát
- Số sự kiện đầu vào và số vé được hệ thống chấp nhận.
- Danh sách EventId được chấp nhận và các mã vé hiển thị gần giống nhau.
- Một mã phân biệt chữ hoa/chữ thường phải tiếp tục được giữ độc lập.
- Kết quả lặp lại phải ổn định qua nhiều lần chạy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi đã thử sửa:** [solution/README.md](solution/README.md)

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
35 phút. Xem [scenario](docs/scenario.md) và [investigation guide](docs/investigation-guide.md) nếu cần thêm bối cảnh.
