# UNIT-SQL-002 — Parameter-Sensitive Query Plan

## Mục tiêu
Điều tra một truy vấn SQL Server có cùng code path nhưng latency và logical reads thay đổi rất lớn tùy tenant. Bạn phải dùng execution plan và runtime evidence để đưa ra hypothesis, sửa và verify mà không làm sai kết quả nghiệp vụ.

## Bối cảnh thực tế
Một API báo cáo đơn hàng phục vụ nhiều tenant. Tenant nhỏ phản hồi ổn định, nhưng một số tenant lớn có p95 tăng mạnh sau khi application warm-up. CPU database không luôn cao và không có exception. Cùng stored procedure có lúc nhanh, có lúc chậm đáng kể.

## Bạn cần làm gì
1. Setup dataset có phân bố dữ liệu lệch giữa các tenant.
2. Chạy workload theo thứ tự được cung cấp và ghi lại logical reads, elapsed time, Actual Execution Plan.
3. Đưa ra ít nhất 3 hypotheses trước khi mở hints.
4. Xác định evidence nào giải thích sự khác biệt.
5. Thử một fix trong `starter/query.sql` hoặc tạo variant riêng.
6. Chạy `verify.ps1` và kiểm tra cả correctness lẫn đặc tính execution.
7. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường
- SQL Server 2022 Developer/Express hoặc SQL Server tương thích local.
- `sqlcmd` có trong PATH.
- PowerShell 7+ khuyến nghị.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` reset plan cache cho database lab, chạy cùng procedure theo hai workload order khác nhau và lưu output vào console. Bật Actual Execution Plan trong SSMS/Azure Data Studio nếu muốn quan sát plan trực quan.

## Những gì cần quan sát
- Result row count có đúng không.
- Logical reads của `Orders`.
- Elapsed time theo tenant và theo thứ tự warm-up.
- Estimated rows so với Actual rows tại các operator quan trọng.
- Join/access strategy có ổn định khi cardinality đầu vào thay đổi không.

Không kết luận chỉ từ một lần timing; ưu tiên evidence về plan, cardinality và reads.

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
[Spoiler — chỉ mở sau khi đã thử](solution/README.md)

## Expected Results
Xem [before](expected-results/before.md) và [after](expected-results/after.md). Không yêu cầu timing tuyệt đối vì máy local khác nhau.

## Estimated Time
Khoảng 45–75 phút.