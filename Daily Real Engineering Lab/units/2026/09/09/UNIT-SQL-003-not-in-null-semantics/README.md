# UNIT-SQL-003 — Customer Exclusion Query Returns Nothing

## Mục tiêu

Điều tra một truy vấn SQL có kết quả đúng với dữ liệu nhỏ nhưng trả về tập rỗng sau khi dữ liệu exclusion xuất hiện giá trị thiếu. Mục tiêu là dùng evidence để hiểu semantics của predicate, sửa truy vấn và giữ nguyên business rule.

## Bối cảnh thực tế

Một scheduled job chọn customer đủ điều kiện nhận renewal reminder. Job phải lấy các customer active nhưng loại những customer đang nằm trong bảng suppression. Sau một đợt import dữ liệu, dashboard cho thấy không còn customer nào được chọn dù phần lớn account vẫn active.

Business impact: reminder không được gửi, renewal conversion giảm và team nghi ngờ import job hoặc database connection.

## Bạn cần làm gì

1. Chạy starter và reproduce kết quả bất thường.
2. Ghi lại input rows và result set thực tế.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Điều tra semantics của predicate trong query.
5. Sửa duy nhất code trong `starter/` để business rule trả về đúng customer.
6. Chạy `./verify.ps1`.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần SQL Server, Docker hay cloud service

Lab dùng SQLite in-memory để tái hiện SQL three-valued logic một cách local-first và deterministic.

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script build và chạy đúng starter state. Reproduction hợp lệ khi chương trình xác nhận result set không khớp business expectation.

## Những gì cần quan sát

- các customer active trong bảng nguồn
- các row trong suppression table
- tập ID mà query trả về
- predicate nào có thể đánh giá thành trạng thái khác ngoài `TRUE`/`FALSE`

Không thay dữ liệu test để làm assertion xanh.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/Program.cs`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ xem sau khi đã tự điều tra và thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- source có nhiều customer active
- suppression chỉ loại một customer hợp lệ
- query lại trả về không có customer đủ điều kiện

Sau khi sửa:

- customer bị suppression vẫn bị loại
- customer active còn lại được trả về chính xác
- logic vẫn đúng khi suppression chứa `NULL`

## Estimated Time

30–45 phút.
