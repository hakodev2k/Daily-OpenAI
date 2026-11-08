# UNIT-PAGECURSOR-001 — Museum export: bản ghi biến mất ở ranh giới phân trang

## Mục tiêu
Điều tra job xuất dữ liệu thiếu hiện vật dù từng trang đều thành công; đưa ra bằng chứng và sửa hợp đồng phân trang.

## Bối cảnh thực tế
Một bảo tàng xuất 5 hiện vật, nhiều bản ghi được cập nhật cùng giây. API trả tối đa 2 bản ghi mỗi trang.

## Bạn cần làm gì
Reproduce, ghi ID từng trang, lập hai hypothesis, kiểm tra cursor, sửa để xuất đúng một lần mỗi ID, diễn giải tương đương SQL.

## Yêu cầu môi trường
.NET SDK 8+, PowerShell (Windows).

## Chạy nhanh
```powershell
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
`dotnet run --project starter/ExportLab.csproj -- --reproduce`.

## Những gì cần quan sát
Số lượng và ID từng trang, ID thiếu, timestamp trùng, điều kiện dừng. Ghi bằng chứng trước khi sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md), [Hint 02](hints/hint-02.md), [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md).

## Expected Results
Starter thiếu bản ghi; sau sửa `./verify.ps1` exit 0, đủ 5 ID không trùng. Thử thêm page size 1/3 và dataset rỗng.

## Estimated Time
45 phút.
