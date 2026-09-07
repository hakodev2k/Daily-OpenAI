# UNIT-SQL-001 — Có index nhưng query vẫn scan gần như toàn bảng

## Mục tiêu
Điều tra một query lọc đơn hàng theo ngày có index trên cột thời gian nhưng execution plan vẫn không tận dụng index theo cách mong đợi.

## Bối cảnh thực tế
Một reporting endpoint truy vấn đơn hàng của một ngày cụ thể. Dữ liệu tăng dần theo thời gian; query ban đầu chạy ổn nhưng chậm rõ rệt khi bảng lớn hơn. DBA xác nhận đã có index trên `CreatedUtc`.

## Bạn cần làm gì
1. Chạy starter và reproduce query plan hiện tại.
2. Ghi ít nhất 2 hypothesis trước khi sửa SQL.
3. So sánh execution plan trước và sau thay đổi.
4. Sửa query trong `starter/Program.cs` mà không thay schema hoặc thêm index mới.
5. Chạy `verify.ps1`.
6. Sau khi verify mới xem hints/solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
Starter tạo một SQLite database tạm, seed dữ liệu, tạo index trên `CreatedUtc`, sau đó in `EXPLAIN QUERY PLAN` và số row khớp.

## Những gì cần quan sát
- Query có trả đúng dữ liệu không.
- Execution plan dùng `SEARCH ... USING INDEX` hay `SCAN`.
- Predicate đang áp dụng trực tiếp lên indexed column hay qua một expression/function.
- Fix có giữ nguyên semantics “toàn bộ một calendar day” hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-1.md)
- [Hint 2](hints/hint-2.md)
- [Hint 3](hints/hint-3.md)

## Reference Solution
⚠️ Spoiler: chỉ mở sau khi đã tự verify.
- [Reference solution](solution/README.md)
- [Wrong fixes](solution/wrong-fixes.md)

## Expected Results
`verify.ps1` phải xác nhận query trả đúng số row và plan chuyển sang index search thay vì full scan.

## Estimated Time
30–45 phút.
