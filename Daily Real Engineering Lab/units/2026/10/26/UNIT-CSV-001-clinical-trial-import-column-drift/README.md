# UNIT-CSV-001 — Báo cáo thử nghiệm lâm sàng bị lệch cột sau khi nhập CSV

## Mục tiêu
Tái hiện lỗi nhập dữ liệu đối tác, thu thập bằng chứng ở cấp bản ghi và trường, sửa đường xử lý dữ liệu, rồi chứng minh không mất hoặc tráo nội dung. Luyện tư duy **input contract → evidence → regression verification**.

## Bối cảnh thực tế
Một hệ thống .NET tiếp nhận báo cáo từ các trung tâm thử nghiệm lâm sàng. File đối tác được chấp nhận theo hợp đồng CSV; mỗi dòng nghiệp vụ có 4 trường: `case_id`, `site`, `participant_note`, `status`. Sau bản cập nhật import, một số hồ sơ bị từ chối vì sai số cột, một số ghi chú hiển thị sai. Không được âm thầm bỏ qua hồ sơ lỗi.

## Bạn cần làm gì
1. Chạy starter và xác nhận triệu chứng bằng số bản ghi, số cột và nội dung trường.
2. Ghi ít nhất hai giả thuyết vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Điều tra `starter/TrialImport/CsvParser.cs`; chỉ chỉnh sửa code trong `starter/`.
4. Chạy verify để kiểm tra cả dữ liệu bình thường, trường hợp biên và input không hợp lệ.
5. So sánh cách sửa của bạn với reference solution sau khi tự hoàn thành.

## Yêu cầu môi trường
- .NET 8 SDK; không cần Docker, database, Azure hoặc NuGet package bên ngoài.
- PowerShell 7 hoặc Windows PowerShell 5.1. Có thể chạy trực tiếp các lệnh `dotnet` bên dưới.

## Chạy nhanh
Từ thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Hoặc:

```powershell
dotnet restore ./starter/TrialImport/TrialImport.csproj
dotnet run --project ./starter/TrialImport/TrialImport.csproj -- demo
dotnet run --project ./starter/TrialImport/TrialImport.csproj -- reproduce
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy **starter gốc**, xuất thống kê và kết thúc exit code 0 khi **triệu chứng đã được chứng minh**. Sau khi sửa, lệnh reproduce sẽ không còn chứng minh được lỗi và có thể trả exit code 1 — đó là điều mong đợi.

## Những gì cần quan sát
- Số bản ghi trả về có khớp số hồ sơ nghiệp vụ trong file?
- Mỗi bản ghi có đúng 4 trường?
- `case_id` và `status` có còn nằm đúng cột?
- Nội dung `participant_note` có được bảo toàn?
- Hành vi với file sai định dạng có rõ ràng hay bị chấp nhận âm thầm?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ **Spoiler:** chỉ mở [solution/README.md](solution/README.md) và [solution/CsvParser.cs](solution/CsvParser.cs) sau khi đã điều tra và thử sửa.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)
- `./verify.ps1` chạy code **trong starter mà bạn đã sửa**, trả exit code 0 khi tất cả kiểm tra thành công.

## Estimated Time
45 phút (L1 / D3).
