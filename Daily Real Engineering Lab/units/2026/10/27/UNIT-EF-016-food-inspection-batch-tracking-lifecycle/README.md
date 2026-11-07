# UNIT-EF-016 — Hồ sơ kiểm tra an toàn thực phẩm cập nhật nhưng trạng thái duyệt không lưu

## Mục tiêu

Điều tra một lỗi dữ liệu xuất hiện sau khi import hàng loạt bằng EF Core, thu thập bằng chứng từ ChangeTracker và SQLite, rồi refactor an toàn cho cả success path và failure path.

## Bối cảnh thực tế

Hệ thống tiếp nhận biên bản kiểm tra an toàn thực phẩm từ nhiều chi nhánh. Sau khi import một batch, nhân viên duyệt một biên bản đã có. UI báo thao tác hoàn tất, nhưng truy vấn mới vẫn thấy trạng thái cũ. Import batch vẫn ghi dữ liệu thành công.

## Bạn cần làm gì

1. Reproduce hiện tượng bằng fixture SQLite độc lập.
2. Ghi lại kết quả SaveChanges, trạng thái thực tế đọc lại từ database và cấu hình DbContext.
3. Đề xuất ít nhất hai hypothesis, xác định điểm bắt đầu sai lệch.
4. Refactor `starter/InspectionImportService.cs` sao cho xử lý được cả exception path và không làm thay đổi cấu hình có sẵn của caller.
5. Chạy verification trên chính thư mục `starter/` đã sửa.

## Yêu cầu môi trường

.NET SDK 8 trở lên, PowerShell 7 hoặc Windows PowerShell 5.1, kết nối NuGet để restore lần đầu. Không cần SQL Server, Azure hay Docker. NuGet package được pin trong csproj.

## Chạy nhanh

Từ thư mục unit:

```powershell
dotnet restore ./starter/InspectionLab.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter chưa sửa và chỉ thành công khi quan sát được sai lệch có chủ đích. Sau khi đã sửa, hãy dùng `verify.ps1`, không dùng reproduce làm tiêu chí pass.

## Những gì cần quan sát

- Batch import trả về bình thường và bản ghi mới tồn tại.
- `SaveChanges()` ở thao tác duyệt tiếp theo trả về bao nhiêu.
- Giá trị trong entity đang tracked và giá trị đọc lại sau `ChangeTracker.Clear()` có khác nhau không.
- Giá trị cấu hình của context trước và sau import, kể cả khi có exception.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ đọc sau khi tự sửa và verify: [Solution](solution/README.md).

## Expected Results

Trước: lệnh duyệt trả về bình thường nhưng SQLite vẫn giữ `Pending`; bài reproduce in `REPRODUCED`.

Sau: verification có 3 case PASS (sau import thành công, sau exception, bảo toàn cấu hình caller); trạng thái database là `Approved`.

## Estimated Time

60–90 phút · L4 / D4 · local-first.
