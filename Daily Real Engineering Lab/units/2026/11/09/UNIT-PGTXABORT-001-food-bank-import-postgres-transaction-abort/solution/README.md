# Reference Solution — chỉ xem sau khi tự reproduce

## Symptoms
D-101 ban đầu có vẻ được insert, D-100 báo unique violation, D-102 báo transaction aborted; sau COMMIT không thấy các dòng mới.

## Evidence
SQLSTATE 23505 là unique_violation. Sau đó 25P02 cho biết transaction hiện tại ở trạng thái lỗi. PostgreSQL không cho chạy thêm statement bình thường trong transaction đó; COMMIT trên transaction lỗi dẫn đến rollback.

## Root cause
Một statement lỗi bên trong transaction làm toàn transaction rơi vào aborted state; bắt lỗi ở client nhưng tiếp tục gửi SQL không tự phục hồi transaction.

## Why the fix works
Xem `reference-import.sql`. Với duplicate dự kiến và được phép bỏ qua, `ON CONFLICT (donation_id) DO NOTHING` tránh unique violation. SAVEPOINT minh họa ranh giới phục hồi cho từng dòng. Với lỗi không dự kiến trong một batch thực tế, ứng dụng phải `ROLLBACK TO SAVEPOINT` trước khi xử lý dòng tiếp theo, hoặc abort toàn batch theo business policy. Script tham chiếu chọn bỏ qua duplicate có chủ đích, không nuốt mọi lỗi.

## Verify
Copy `solution/reference-import.sql` vào `starter/import.sql` **chỉ sau khi tự thử**; chạy `./verify.ps1`. Tổng kỳ vọng: 3 dòng, 6800 cents. Lệnh verify reset fixture, thực thi learner-editable starter và assert chính xác ID/amount.

## Alternative fixes
1. Transaction riêng từng dòng: cô lập lỗi, nhưng mất tính atomic của toàn batch và cần reconciliation.
2. Staging table + validation + merge: thích hợp dữ liệu lớn, hỗ trợ báo cáo hàng lỗi.
3. Client catch + ROLLBACK TO SAVEPOINT: phù hợp khi phải phân loại nhiều SQLSTATE.

## Wrong / tempting fixes
- Chỉ catch exception rồi tiếp tục INSERT: transaction vẫn aborted.
- Tắt unique constraint: tạo double-count tài chính.
- Retry toàn batch không có idempotency: có thể tăng tải hoặc ghi trùng nếu boundary không rõ.
- Dùng ON CONFLICT DO NOTHING cho mọi bảng: có thể che mất dữ liệu nguồn khác amount; phải đối soát duplicate khác nội dung.

## Production implications
Ghi audit riêng cho mã trùng, kiểm tra amount mismatch, đo tỷ lệ duplicate và transaction rollback, đảm bảo không log PII. Chọn chính sách rõ ràng cho file invalid, partial import, và checkpoint.

## Trade-offs
SAVEPOINT có chi phí mỗi dòng; staging/set-based import thường tốt hơn với file lớn. ON CONFLICT đơn giản nhưng phải có unique key đúng business identity. Khi yêu cầu all-or-nothing tuyệt đối, không được bỏ qua dòng lỗi.

## Senior engineer should notice
Phân biệt expected duplicate và lỗi hệ thống, transaction state và business acceptance; test cả trường hợp conflict cùng ID nhưng khác amount, không chỉ happy path.

## References
- PostgreSQL: https://www.postgresql.org/docs/16/sql-savepoint.html
- PostgreSQL: https://www.postgresql.org/docs/16/sql-rollback-to.html
- PostgreSQL: https://www.postgresql.org/docs/16/errcodes-appendix.html
- PostgreSQL: https://www.postgresql.org/docs/16/sql-insert.html
