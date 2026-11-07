# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Endpoint chỉ cần tối đa 20 bản ghi nhưng starter materialize toàn bộ 5.000 `Customer` trước khi lọc.

## 2. Evidence
SQL log của starter cho thấy query lấy toàn bộ entity columns và không chứa business filter/limit. Output xác nhận `Materialized=5000; Returned=20`.

## 3. Root cause
`ToList()` là terminal operation. Nó thực thi `IQueryable<Customer>` quá sớm; các operator sau đó chạy bằng LINQ-to-Objects. Vì vậy database không còn cơ hội áp dụng filter, projection và limit.

## 4. Why the fix works
Giữ `Where`, `Select` và `Take` trong `IQueryable` cho tới terminal operation cho phép EF Core translate chúng thành SQL. `AsNoTracking()` phù hợp với read-only query và tránh change-tracking overhead.

## 5. How to verify
Chạy `verify.ps1`, đồng thời đọc SQL log. Query phải trả 20 rows và SQL phải chứa filtering/limit thay vì select toàn bộ dataset.

## 6. Alternative fixes
Compiled query có thể hữu ích với hot query sau khi đã đo overhead, nhưng không sửa được một query shape sai. Pagination/seek có thể cần khi endpoint thực sự hỗ trợ nhiều trang.

## 7. Wrong or misleading fixes
Tăng memory hoặc scale-out chỉ che chi phí query. Thêm index trước khi sửa query boundary có thể không giải quyết lượng columns/entities materialized. Cache toàn bộ customer list làm tăng consistency và invalidation complexity.

## 8. Production implications
Early materialization làm tăng network transfer, allocations, GC pressure và tracking cost; tác động tăng theo data size dù response size cố định.

## 9. Trade-offs
Projection read models giảm dữ liệu và tracking nhưng cần chọn fields rõ ràng. `AsNoTracking()` phù hợp read-only path, không nên dùng mù quáng khi operation cần entity tracking.

## 10. What a Senior engineer should notice
Cần kiểm tra query boundary và generated SQL trước khi tối ưu bằng infrastructure. Phân biệt `IQueryable` với `IEnumerable` là một phần của performance correctness trong EF Core.