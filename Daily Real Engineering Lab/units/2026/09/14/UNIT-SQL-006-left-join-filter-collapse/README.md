# UNIT-SQL-006 — LEFT JOIN Result Set Collapse

## Mục tiêu

Điều tra một reporting query cần giữ lại toàn bộ warehouse nhưng thực tế chỉ trả về warehouse có inventory row phù hợp.

## Bối cảnh thực tế

Một operations dashboard phải hiển thị mọi warehouse, kể cả warehouse chưa có inventory snapshot active. Sau một thay đổi query, dashboard không báo lỗi nhưng một số warehouse biến mất hoàn toàn khỏi result set.

## Bạn cần làm gì

- Reproduce hiện tượng bằng starter project.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- So sánh dữ liệu nguồn với result set.
- Sửa query trong `starter/Program.cs` để giữ đúng contract của report.
- Chạy `verify.ps1` để xác nhận regression case.
- Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Internet chỉ cần thiết ở lần restore NuGet đầu tiên

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/Program.cs`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter và xác nhận report đang thiếu warehouse so với contract nghiệp vụ.

## Những gì cần quan sát

- Có bao nhiêu warehouse trong bảng nguồn.
- Warehouse nào có snapshot active, inactive hoặc chưa có snapshot.
- Bao nhiêu warehouse xuất hiện trong report.
- Query hoàn thành bình thường và không throw exception.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- report chỉ còn một phần warehouse.
- không có SQL exception.

After:
- report giữ đủ warehouse `1,2,3`.
- warehouse có snapshot active nhận quantity tương ứng.
- warehouse không có snapshot active vẫn xuất hiện với quantity rỗng.

Nếu không reproduce được, chạy `dotnet --info`, sau đó xóa `starter/bin` và `starter/obj` rồi chạy lại script.

## Estimated Time

30–45 phút.
