# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce, tự điều tra và thử sửa.**

## 1. Symptoms
Truy vấn trả đúng 5.000 ID nhưng `Heap Fetches` tăng sau metadata sync dù covering index không thay đổi.

## 2. Evidence
`reproduce.ps1` cho thấy trước sync `Heap Fetches = 0`, sau sync lớn hơn 0. `Index Only Scan` vẫn xuất hiện, cùng số rows. Bảng có cấu hình maintenance đặc biệt.

## 3. Root cause
PostgreSQL chỉ tránh heap access trong index-only scan nếu visibility map đánh dấu heap page all-visible cho snapshot. UPDATE xóa all-visible bit của các page bị ảnh hưởng. Tắt autovacuum khiến trạng thái này không được tự phục hồi. Index chứa đủ cột nhưng PostgreSQL vẫn phải kiểm tra visibility của tuples ở heap.

## 4. Why the fix works
[reference.sql](reference.sql) bật autovacuum với threshold nhỏ cho fixture và thực hiện `VACUUM (ANALYZE)` đồng bộ để cập nhật visibility map. Bài lab dùng VACUUM thủ công để verify ngay; production cần bảo trì liên tục.

## 5. How to verify
Sau khi tự thử, có thể chép reference SQL sang `workspace/remediation.sql` để so sánh. `verify.ps1` tái tạo lỗi, thực thi **workspace**, kiểm tra `Heap Fetches = 0` và không mất 5.000 rows. Reference không được chạy ngầm.

## 6. Alternative fixes
Chạy VACUUM theo maintenance window khi workload thích hợp; giảm UPDATE không cần thiết; chấp nhận heap fetches nếu write-heavy và chi phí maintenance lớn hơn lợi ích đọc. Đo thực tế trước khi chọn.

## 7. Wrong / tempting fixes
- Thêm một index bao phủ tương tự: không sửa visibility map, tăng write amplification.
- Ép index-only scan bằng planner settings: tên node không chứng minh không đọc heap.
- Chạy VACUUM một lần rồi bỏ mặc: symptom tái phát sau UPDATE tiếp theo.
- VACUUM FULL ngay: khóa và chi phí lớn, không phải biện pháp mặc định.

## 8. Production implications
Theo dõi `pg_stat_user_tables`, `last_autovacuum`, `n_dead_tup`, heap fetches, I/O, WAL và bloat. Không sao chép threshold từ fixture sang production nếu chưa có số đo.

## 9. Trade-offs
Vacuum tốn I/O/CPU nhưng cải thiện visibility map và xử lý dead tuples. Churn cao có thể khiến heap fetches vẫn tồn tại giữa các lần vacuum; index-only scan không phải mục tiêu tuyệt đối.

## 10. Senior engineer should notice
Tách semantics của query plan khỏi đo latency, kiểm tra runtime storage state thay vì chỉ DDL, so sánh trước/sau trên cùng fixture và phân biệt remediation tức thời với policy dài hạn.

## Official references
- [Index-Only Scans and Covering Indexes](https://www.postgresql.org/docs/16/indexes-index-only-scans.html)
- [Routine Vacuuming](https://www.postgresql.org/docs/16/routine-vacuuming.html)
- [EXPLAIN](https://www.postgresql.org/docs/16/sql-explain.html)
