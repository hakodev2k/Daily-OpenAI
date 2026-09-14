# UNIT-SQL-006 — Missing Rows in Exclusion Query

## Mục tiêu

Điều tra một SQL query dùng để lấy danh sách customer đủ điều kiện xử lý nhưng trả về tập kết quả thiếu nghiêm trọng dù dữ liệu nhìn qua có vẻ hợp lệ.

## Bối cảnh thực tế

Một scheduled job gửi thông báo cho customer chưa bị block. Sau một thay đổi dữ liệu ở bảng block list, job vẫn chạy thành công nhưng không còn chọn được các customer đáng lẽ phải được xử lý. Không có exception và query hoàn thành rất nhanh.

## Bạn cần làm gì

- Reproduce hiện tượng bằng starter project.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Quan sát dữ liệu nguồn và kết quả của exclusion query.
- Sửa query trong `starter/Program.cs` để chỉ loại đúng customer bị block.
- Chạy `verify.ps1` để xác nhận cả correctness lẫn regression case.
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

`reproduce.ps1` restore/run starter và xác nhận query hiện tại không trả về tập customer mong đợi.

## Những gì cần quan sát

- Các customer đang tồn tại.
- Dữ liệu trong block list.
- Tập ID thực tế query trả về.
- Tập ID nghiệp vụ mong đợi.
- Query không throw exception dù kết quả sai.

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
- chương trình chứng minh exclusion query không trả về `[1,3,4]`.
- không có SQL exception.

After:
- query trả về chính xác `[1,3,4]`.
- customer `2` vẫn bị loại.
- dữ liệu phụ trong block list không còn làm mất các customer hợp lệ.

Nếu không reproduce được, chạy `dotnet --info`, sau đó xóa `starter/bin` và `starter/obj` rồi chạy lại script.

## Estimated Time

30–45 phút.
