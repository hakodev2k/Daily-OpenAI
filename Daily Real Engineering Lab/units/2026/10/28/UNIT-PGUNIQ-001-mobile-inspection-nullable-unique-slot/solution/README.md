# Reference Solution — chỉ xem sau khi tự verify

## 1. Triệu chứng
Hai booking lưu động tại depot 42 cùng 08:00 UTC được commit, trong khi duplicate tại bay 7 bị từ chối.

## 2. Evidence
starter/02-reproduce.sql in hai dòng có depot_id=42, vehicle_bay_id=NULL, starts_at=08:00. UNIQUE hiện tại vẫn chặn hai dòng có vehicle_bay_id=7.

## 3. Root cause
PostgreSQL UNIQUE mặc định coi NULL khác NULL cho mục đích uniqueness. Vì vậy hai tuple (42, NULL, 08:00) không vi phạm constraint, dù nghiệp vụ coi NULL là cùng nhóm kiểm định lưu động. Đây là bất đồng giữa data model và business invariant, không phải lỗi race condition của ứng dụng.

## 4. Fix và vì sao hiệu quả
Đổi khai báo thành:

~~~sql
CONSTRAINT uq_inspection_slot
    UNIQUE NULLS NOT DISTINCT (depot_id, vehicle_bay_id, starts_at)
~~~

PostgreSQL 15+ coi NULL là không phân biệt trong ràng buộc này; duplicate của nhóm lưu động bị chặn ngay ở database. Bản đầy đủ nằm trong solution/01-schema.sql. Chỉ sửa starter/01-schema.sql để verify.ps1 kiểm tra đúng phần bạn làm.

## 5. Verify
Chạy pwsh ./verify.ps1 sau khi chỉnh starter. Regression test yêu cầu hai conflict (mobile và fixed bay) bị từ chối; ba lịch khác depot/bay/thời điểm phải được chấp nhận. Script rollback transaction để test lặp lại được. Để kiểm tra bản tham chiếu riêng, nạp solution/01-schema.sql bằng psql rồi chạy starter/03-verify.sql; không dùng reference để thay thế learner verification.

## 6. Alternative fixes
- Partial unique index chỉ cho vehicle_bay_id IS NULL kết hợp với constraint cho bay cố định: hợp lệ nhưng nhiều index hơn và khó bảo trì.
- Unique expression index với COALESCE(vehicle_bay_id, sentinel): có thể hợp lệ nếu sentinel bị cấm tuyệt đối và chính sách domain rõ ràng; cần tránh collision và xử lý migration.
- Nếu NULL thực sự có nghĩa 'unknown bay' thay vì 'shared mobile bay', phải thiết kế lại domain trước khi áp dụng NULLS NOT DISTINCT.

## 7. Wrong / tempting fixes
- SELECT trước INSERT: hai request đồng thời vẫn có thể cùng thấy slot trống; thiếu atomicity.
- Bắt lỗi ở frontend: không bảo vệ các nguồn ghi khác hoặc import batch.
- Tạo index chỉ trên depot_id, starts_at: chặn nhầm các bay cố định khác nhau cùng thời điểm.
- Chỉ thêm retry: không sửa invariant; có thể làm tăng contention.

## 8. Production implications
- Kiểm tra dữ liệu hiện tại trước migration, ví dụ:

~~~sql
SELECT depot_id, vehicle_bay_id, starts_at, count(*)
FROM lab.inspection_slots
GROUP BY depot_id, vehicle_bay_id, starts_at
HAVING count(*) > 1;
~~~

- Với các nhóm trùng, xác minh lịch nào hợp lệ cùng vận hành; không xóa dữ liệu tự động. Dọn dữ liệu theo quy trình audit trước khi tạo constraint mới.
- Lên kế hoạch locking, thời gian tạo index/constraint, triển khai ở môi trường staging, rollback và giám sát unique_violation; trong production không dùng DROP SCHEMA.
- Ứng dụng phải xử lý SQLSTATE 23505 thành lỗi conflict nghiệp vụ thay vì 500, nhưng database vẫn là authoritative guard.

## 9. Trade-offs
UNIQUE NULLS NOT DISTINCT ngắn gọn, dễ audit và hỗ trợ concurrency; yêu cầu PostgreSQL 15+. Partial indexes hỗ trợ phiên bản cũ hơn nhưng phức tạp. Việc tạo ràng buộc trên bảng lớn có chi phí vận hành và có thể thất bại nếu tồn tại duplicates.

## 10. Senior engineer cần nhận ra
Semantics của NULL phải được quyết định theo nghiệp vụ trước khi chọn constraint. Một unit test chỉ kiểm tra bay_id có giá trị sẽ bỏ sót nhánh quan trọng. Cần test tại database, bao gồm ghi đồng thời và dữ liệu lịch sử, đồng thời lập migration không làm mất bằng chứng kiểm định.

## References
PostgreSQL 16: https://www.postgresql.org/docs/16/ddl-constraints.html#DDL-CONSTRAINTS-UNIQUE-CONSTRAINTS
PostgreSQL CREATE TABLE: https://www.postgresql.org/docs/16/sql-createtable.html
