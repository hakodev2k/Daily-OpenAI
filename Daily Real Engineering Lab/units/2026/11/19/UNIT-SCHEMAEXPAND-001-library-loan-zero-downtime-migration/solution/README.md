# Reference Solution — chỉ xem sau khi tự điều tra
## Root cause
M0 kết hợp thay đổi phá tương thích với triển khai rolling; ADD NOT NULL DEFAULT trên bảng lớn có thể cần schema-modification lock và có thể chờ reader, trong khi UNIQUE trên Loan.BorrowerKey sai mô hình cardinality: một người có nhiều Loan. Giá trị mặc định UNKNOWN tạo duplicate ngay lập tức. V2 chạy trước khi migration thành công dẫn tới invalid column. CPU thấp không bác bỏ lock contention.
## Plan tham chiếu
1. **S1 expand:** tạo bảng BorrowerIdentity(BorrowerKey unique, LegacyCode mapping có chính sách xử lý collision), thêm Loan.BorrowerKey nullable và index phục vụ backfill; DDL chạy trong window có LOCK_TIMEOUT thấp, đo lock wait, tránh long transaction. V1 tiếp tục dùng BorrowerCode.
2. **S2 compatibility:** triển khai V2 có feature flag, đọc BorrowerKey nếu có, fallback sang BorrowerCode; ghi cả hai trong transaction tương thích. Khi V1 vẫn ghi, worker backfill lấy batches nhỏ, checkpoint theo LoanId và cập nhật có điều kiện `WHERE BorrowerKey IS NULL` sau khi resolve mapping; catch-up quét lại rows mới, không coi high-watermark đơn thuần là đủ. Nếu mapping không xác định, quarantine và dừng enforcement.
3. **S3 cutover:** xác minh count NULL=0 và mapping đúng, không có V1 writers; theo dõi lag <=5 phút, p95 <=250 ms, error <0.5%. Bật V2 100% nhưng giữ old column và dual-write trong rollback window. Rollback application bằng feature flag/V1; giữ schema mở rộng, không rollback DDL bằng DROP.
4. **S4 contract:** chỉ sau expiry rollback window, xác nhận không còn V1, đổi constraint bằng migration riêng với lock budget; drop old column trong release khác. Không unique Loan.BorrowerKey vì một người có nhiều loans.
## Failure cards
A: worker restart → checkpoint + idempotent batches, restart không làm mất updates.
B: V1 writes trong backfill → catch-up scan/CDC hoặc trigger có kiểm soát; đo backlog trước cutover.
C: V2 rollback → giữ BorrowerCode được ghi nhất quán cho V1; schema mở rộng vẫn tồn tại.
D: schema lock 20 s → cancel DDL theo lock timeout và retry ở window thấp tải; không tăng timeout vô hạn.
E: 0.2% unresolvable → quarantine và xử lý mapping; NO-GO enforcement.
## Verification
Kiểm tra `SELECT COUNT(*) FROM dbo.Loan WHERE BorrowerKey IS NULL`, group theo BorrowerKey để tìm mapping bất thường (duplicate Loan keys có thể hợp lệ), kiểm tra referential integrity với BorrowerIdentity, so sánh count writes theo phiên bản, p95/error và lock waits. Backfill completion không chỉ là số hàng đã scan.
## Wrong fixes
- Chỉ tăng SQL command timeout: kéo dài request waiting, không sửa schema lock hay compatibility.
- Thêm unique index trực tiếp trên Loan.BorrowerKey: vi phạm quan hệ một-nhiều.
- Dừng tất cả V1 ngay khi migration bắt đầu: không phù hợp rolling và rollback.
- DROP BorrowerCode ngay sau backfill: phá rollback, có thể làm mất dữ liệu V1 cần.
## Trade-offs
Dual-write làm tăng độ phức tạp, cần telemetry divergence; CDC/trigger có overhead và vận hành; batch size cân bằng lock/log/lag; schema contract trì hoãn tạo chi phí tạm thời nhưng giảm blast radius.
## Further reading
Microsoft Learn: SQL Server transaction locking guide và SQL Server index design guide; tài liệu EF Core migrations và deployment. Tra cứu phiên bản tài liệu phù hợp trước khi áp dụng production.
