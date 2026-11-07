# UNIT-LINQ-007 — Bộ lọc báo cáo thay đổi ngoài dự kiến

## Mục tiêu
Điều tra một lỗi C#/LINQ nơi cùng một query object cho kết quả khác nhau giữa hai thời điểm dù code tạo query không thay đổi.

## Bối cảnh thực tế
Một scheduled report tạo tập dữ liệu cần xuất, sau đó một bước khác cập nhật biến cấu hình trong cùng operation. Log cho thấy query ban đầu đúng, nhưng lúc enumerate để export lại chứa thêm bản ghi ngoài phạm vi dự kiến.

## Bạn cần làm gì
1. Reproduce trước khi sửa.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Quan sát thời điểm query được tạo, thời điểm enumerate và giá trị filter ở từng thời điểm.
4. Sửa `starter/` sao cho tập dữ liệu export giữ đúng contract nghiệp vụ.
5. Chạy `verify.ps1`.
6. Chỉ sau đó xem `solution/README.md`.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- số item dự kiến tại thời điểm chuẩn bị report
- số item thực tế khi export
- giá trị filter được log ở các phase
- query object có được enumerate ngay khi khai báo hay không

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: reproduce script xác nhận export quan sát tập dữ liệu khác với snapshot nghiệp vụ mong đợi.

After: verify script xác nhận report giữ đúng tập dữ liệu đã được xác định tại report boundary, đồng thời các thay đổi cấu hình sau đó không làm thay đổi report đang xử lý.

Nếu không reproduce được, chạy `dotnet --version` rồi `dotnet run --project starter -- --reproduce`.

## Estimated Time
30–45 phút.