# UNIT-LOG-001 — Bảo hành linh kiện: log đọc được nhưng không truy vấn được

## Mục tiêu
Điều tra một sự cố observability của .NET, tìm bằng chứng vì sao cảnh báo hiển thị đúng nhưng công cụ tìm kiếm không lọc được theo trường nghiệp vụ.

## Bối cảnh thực tế
Cổng tiếp nhận bảo hành linh kiện hàng không phát cảnh báo khi một linh kiện cần kiểm tra. Đội vận hành tìm theo `PartNumber` và `ClaimId` để lập danh sách thu hồi. Sau một đợt thay đổi, nội dung cảnh báo vẫn đúng nhưng bộ lọc trả về 0 kết quả.

## Bạn cần làm gì
Chạy starter; ghi lại triệu chứng và ít nhất hai giả thuyết; điều tra dữ liệu provider thực nhận; sửa **chỉ** `starter/Program.cs` mà không đổi nội dung cảnh báo; xác nhận kết quả.

## Yêu cầu môi trường
.NET SDK 8+ có thể target `net8.0`; PowerShell. Không cần database, Docker, Azure hoặc NuGet ngoài framework `Microsoft.AspNetCore.App`.

## Chạy nhanh
```powershell
dotnet --version
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` **trước khi sửa starter**. Kết quả `REPRODUCED` với exit code 0 xác nhận triệu chứng ban đầu, không xác nhận đã fix.

## Những gì cần quan sát
So sánh `Rendered warning` và `Indexed attributes`. Ghi lại trường nào xuất hiện trong thông điệp nhưng không có trong thuộc tính có thể truy vấn. Nếu không reproduce được, kiểm tra bạn chưa sửa starter và chạy `dotnet run --project starter/ClaimLogLab.csproj -- --reproduce`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Chạy `./verify.ps1` trên **starter đã sửa**.
5. Sau đó mới xem reference.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [solution/README.md](solution/README.md) chỉ mở sau khi tự thử sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50–65 phút (L3 / D4).
