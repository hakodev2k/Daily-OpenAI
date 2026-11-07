# Reference Solution — chỉ đọc sau khi đã tự reproduce và thử fix

## 1. Symptoms
Job báo không có hợp đồng thiếu xác nhận, trong khi `E-102` và `E-103` vẫn ACTIVE và không xuất hiện trong batch 42.

## 2. Evidence
Bốn enrollment ACTIVE của tenant A: `E-101,E-102,E-103,E-105`. Xác nhận batch 42 của A: `E-101,E-105,NULL`. Xác nhận `E-102` nằm ở batch 41, `E-103` thuộc tenant B.

## 3. Root cause
SQL dùng logic ba giá trị (`TRUE`, `FALSE`, `UNKNOWN`). Với `x NOT IN (subquery)`, một phần tử `NULL` trong tập có thể khiến mọi so sánh "không khớp" cho kết quả `UNKNOWN`. Mệnh đề `WHERE` chỉ giữ `TRUE`, nên hai enrollment chưa xác nhận bị loại khỏi kết quả. Đây không phải vấn đề ingest hay index.

## 4. Why the fix works
[Xem SQL tham chiếu](query.sql). `NOT EXISTS` kiểm tra sự tồn tại của **dòng khớp thực sự** theo tenant, batch và enrollment. Dòng có `enrollment_id IS NULL` không khớp bằng phép `=` với mã hợp đồng, nên không làm nhiễm điều kiện "không tồn tại".

## 5. How to verify
Sửa `starter/query.sql` theo logic trên, chạy `./scripts/verify.ps1`. Assertion so sánh tập mã `E-102,E-103`, tổng số dòng = 2 và không có xác nhận trùng khóa nghiệp vụ trong batch 42.

## 6. Alternative fixes
Lọc `WHERE c.enrollment_id IS NOT NULL` ngay trong subquery `NOT IN` cũng cho kết quả đúng với bộ dữ liệu này, nếu đảm bảo đầy đủ scope và semantics. `LEFT JOIN ... WHERE c.confirmation_id IS NULL` là một anti-join khác; phải đặt điều kiện tenant/batch trong `ON` và chọn cột non-null để nhận biết thiếu match.

## 7. Wrong / tempting fixes
- Bỏ toàn bộ bản ghi lỗi khỏi ingest: có thể cần data-quality quarantine, nhưng không nên làm báo cáo mất hợp đồng chỉ vì upstream gửi một dòng thiếu mã.
- Chỉ thêm `DISTINCT`: không giải quyết `UNKNOWN`.
- Bỏ tenant/batch filter để "lấy đủ dòng": tạo false negative hoặc lẫn dữ liệu đối tác.
- Thêm index hoặc tăng timeout: không sửa logic.
- Chuyển `NOT IN` thành `IN`: đảo sai ý nghĩa nghiệp vụ.

## 8. Production implications
Đo tỉ lệ xác nhận không có enrollment_id; log và quarantine theo batch mà không ghi dữ liệu nhạy cảm. Bổ sung regression test cho NULL, tenant khác, batch cũ, inactive, duplicate và batch trống. Với bảng lớn, đánh giá index `(tenant_id,batch_id,enrollment_id)` trên confirmations và `(tenant_id,status,enrollment_id)` trên enrollments bằng `EXPLAIN (ANALYZE, BUFFERS)` trên môi trường dữ liệu phù hợp.

## 9. Trade-offs
`NOT EXISTS` diễn đạt anti-semi-join rõ ràng và an toàn với nullable key. `LEFT JOIN` cũng hợp lệ nhưng dễ đặt nhầm predicate; `NOT IN` với explicit NULL filtering ngắn gọn nhưng cần duy trì bất biến của dữ liệu và truy vấn. Không thêm index theo cảm tính khi chưa đo.

## 10. Senior engineer should notice
Bài toán có hai contract độc lập: chất lượng payload của đối tác và tính đúng đắn của query dưới dữ liệu lỗi. Phải bảo vệ tính đúng đắn ở query, đồng thời quan sát chất lượng ingest; một báo cáo "0 vấn đề" sai còn nguy hiểm hơn báo cáo lỗi rõ ràng.

## References
- PostgreSQL 16: [Subquery expressions](https://www.postgresql.org/docs/16/functions-subquery.html)
- PostgreSQL 16: [Row and array comparisons](https://www.postgresql.org/docs/16/functions-comparisons.html)
