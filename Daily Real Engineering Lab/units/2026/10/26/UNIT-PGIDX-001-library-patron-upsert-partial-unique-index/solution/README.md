# Reference Solution — Spoiler

## Root cause
PostgreSQL chỉ có partial unique index trên `(branch_id, external_card) WHERE deleted_at IS NULL`. `ON CONFLICT (branch_id, external_card)` không cung cấp predicate để suy luận đúng partial unique index, nên PostgreSQL báo không có unique/exclusion constraint phù hợp. Không phải do index chưa được tạo hay có duplicate.

## Fix
Xem [upsert.sql](upsert.sql): bổ sung `WHERE deleted_at IS NULL` **trong conflict target** trước `DO UPDATE`. Nhờ đó PostgreSQL infer được partial unique index và chỉ coi các active rows là conflict. Câu lệnh vẫn giữ atomic upsert của một statement.

## Cách so sánh
Copy nội dung `solution/upsert.sql` sang `starter/upsert.sql` trong clone local, chạy `./verify.ps1`. Harness không tham chiếu trực tiếp solution.

## Wrong fixes
- **DROP partial index rồi tạo UNIQUE không predicate:** phá contract giữ lịch sử thẻ đã xóa.
- **SELECT rồi INSERT/UPDATE:** race giữa các worker import; không thay thế được atomic upsert.
- **`ON CONFLICT DO NOTHING`:** che lỗi và không cập nhật tên chủ thẻ active khi nhập lại.
- **Xóa tombstone trước khi import:** mất audit history, trái yêu cầu nghiệp vụ.
- **Bắt lỗi rồi retry cùng SQL:** không giải quyết lỗi inference cố định.

## Trade-offs
- Partial unique index cần conflict target predicate tương thích; phải kiểm tra cùng migration và DML.
- Nếu chính sách soft-delete đổi, test contract trước khi deploy.
- Ở production cần kiểm tra concurrent upserts, isolation level, deadlocks và dữ liệu đã tồn tại; lab tập trung vào index inference và deterministic fixtures.
