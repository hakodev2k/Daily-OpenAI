# UNIT-EF-019 — Báo cáo chuyển tuyến thiếu sự kiện sau khi hồ sơ bệnh nhân được lưu trữ

## Mục tiêu
Điều tra một sự cố **mất dòng trong báo cáo audit** của hệ thống chuyển tuyến phòng khám, dùng dữ liệu và câu SQL thực tế để phân biệt thiếu dữ liệu gốc với sai lệch khi truy vấn. Đề xuất cách sửa không làm lộ thông tin hồ sơ đã lưu trữ.

## Bối cảnh thực tế
Nhóm tuân thủ nhận thấy báo cáo lịch sử của tenant `TENANT-ALPHA` chỉ xuất **1 sự kiện**, trong khi nhật ký ghi nhận **2 sự kiện hợp lệ**. API trả HTTP 200, không có exception, và bản ghi trong database vẫn còn. Sự cố xuất hiện sau một thao tác lưu trữ hồ sơ liên quan.

## Bạn cần làm gì
1. Chạy bản starter và ghi lại số dòng, danh sách ID và SQL.
2. Kiểm tra ba giả thuyết: thiếu dữ liệu, lọc tenant, hoặc hình dạng truy vấn.
3. Sửa duy nhất đường xuất báo cáo trong `starter/Program.cs`; không xóa các chính sách truy cập dữ liệu.
4. Đảm bảo báo cáo trả đúng hai sự kiện của tenant, không trả tenant khác và không materialize hồ sơ bệnh nhân.
5. So sánh bằng chứng trước/sau và ghi trade-off trong `workspace/my-investigation.md`.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7.
- .NET 8 SDK (xem `global.json`); NuGet restore lần đầu cần kết nối tải package đã pin.
- Không cần Azure, SQL Server hay dịch vụ trả phí: SQLite in-memory được tạo mỗi lần chạy.

## Chạy nhanh
Tại thư mục unit:

```powershell
./run.ps1
./reproduce.ps1
# Sau khi sửa starter:
./verify.ps1
```

Hoặc: `dotnet run --project starter/AuditExport.csproj -- --run`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên **starter chưa sửa**. Script trả exit code 0 và in `REPRODUCED` khi quan sát được đúng mẫu sai lệch dự kiến; đây **không** phải test đạt yêu cầu nghiệp vụ. Nếu không reproduce được, dừng và ghi lại console/SQL.

## Những gì cần quan sát
- Số sự kiện trong database và số dòng được export có khớp nhau không?
- SQL có thực hiện JOIN không, và loại JOIN có thể ảnh hưởng đến số dòng như thế nào?
- Có record của tenant khác xuất hiện không?
- Một thay đổi vòng đời của hồ sơ liên quan có làm biến mất lịch sử sự kiện hay không?
- `./verify.ps1` phải trả exit code 0 sau khi sửa, dựa trên **starter đang được chỉnh sửa**, không chạy solution.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi ít nhất hai hypotheses và bằng chứng.
3. Sửa tối thiểu, không phá vỡ ranh giới tenant và chính sách bảo mật.
4. Verify, đọc lại SQL, rồi so sánh giải pháp tham chiếu.
5. Không dùng `IgnoreQueryFilters()` một cách mù quáng.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi đã điều tra:** [solution/README.md](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
70 phút (L3 / D4).
