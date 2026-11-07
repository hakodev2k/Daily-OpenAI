# UNIT-SQL-011 — Báo cáo sự cố thiết bị thư viện bỏ sót chi nhánh

## Mục tiêu

Điều tra một báo cáo SQL có tổng số sự cố nghiêm trọng đúng nhưng danh sách chi nhánh không đầy đủ; refactor truy vấn và xây dựng regression contract để các chi nhánh không có sự cố vẫn được báo cáo chính xác.

## Bối cảnh thực tế

Hệ thống thư viện công cộng theo dõi sự cố thiết bị tự phục vụ ở từng chi nhánh. Dashboard cần hiển thị **mọi chi nhánh** và số sự cố `Open` + `Critical` của từng nơi, kể cả khi con số bằng 0. Sau khi bổ sung bộ lọc cho báo cáo, các chi nhánh không có cảnh báo nghiêm trọng biến mất; tổng sự cố của thành phố vẫn khớp với hệ thống giám sát.

## Bạn cần làm gì

1. Chạy starter, ghi số dòng và dữ liệu quan sát được; không mở lời giải.
2. Đề xuất ít nhất ba giả thuyết: dữ liệu seed, cách lọc, phép đếm, hoặc quan hệ giữa các bảng.
3. Đọc `starter/BranchReportRepository.cs`, đối chiếu với dữ liệu được tạo ở `checks/Program.cs`.
4. Chỉ sửa `starter/BranchReportRepository.cs`; giữ nguyên checks, scripts và schema.
5. Chạy verification; giải thích vì sao chi nhánh không có bản ghi, chỉ có sự cố đã đóng, hoặc chỉ có sự cố nhẹ đều phải hiện với giá trị 0.

## Yêu cầu môi trường

.NET SDK 8 hoặc mới hơn, PowerShell 5.1/7. `Microsoft.Data.Sqlite` 8.0.11 được pin trong project; lần restore đầu tiên cần NuGet. Không cần Docker, Azure hoặc database bên ngoài.

## Chạy nhanh

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi sửa starter:

```powershell
./verify.ps1
```

Hoặc chạy `dotnet run --project checks/Checks.csproj -- reproduce` và `-- verify` trên macOS/Linux.

## Cách reproduce vấn đề

`reproduce.ps1` tạo SQLite database in-memory với bốn chi nhánh và nhiều trạng thái sự cố. Script trả exit code 0 khi xác nhận được triệu chứng đã biết của starter; trả non-zero nếu dữ liệu hoặc hành vi không khớp. Chạy script này **trước** khi sửa.

## Những gì cần quan sát

- Số chi nhánh thực tế trong bảng so với số hàng trong dashboard.
- Tổng số sự cố `Open` + `Critical` trong bảng so với tổng trên dashboard.
- Các chi nhánh có 0 sự cố, chỉ sự cố `Closed`, hoặc chỉ `Minor` có được hiển thị hay không.
- Kết quả sau khi thêm một chi nhánh mới và sau khi đổi trạng thái sự cố.

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

**Spoiler — chỉ đọc sau khi tự điều tra và thử sửa:** [Reference Solution](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

60–75 phút. Ghi nhận bằng chứng vào [workspace/my-investigation.md](workspace/my-investigation.md).
