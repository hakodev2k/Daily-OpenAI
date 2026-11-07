# Sau lab — SQLite write semantics

SQLite conflict-resolution REPLACE và UPSERT ON CONFLICT DO UPDATE không tương đương. REPLACE khi gặp UNIQUE/PRIMARY KEY conflict có thể xóa hàng cũ rồi chèn hàng mới. Với ON DELETE CASCADE, dữ liệu con có thể bị xóa hợp lệ về mặt foreign key nhưng sai về mặt nghiệp vụ.

Một regression contract phải kiểm tra cả dữ liệu cần giữ nguyên, không chỉ việc command trả thành công. PRAGMA foreign_key_check không phát hiện việc cascade-delete hợp lệ nhưng ngoài ý muốn.

Tài liệu chính thức:
- https://www.sqlite.org/lang_conflict.html
- https://www.sqlite.org/lang_upsert.html
- https://www.sqlite.org/foreignkeys.html

Mở rộng: nếu nhiều offline clients đồng bộ cùng Equipment, version predicate, transaction boundary và audit trail cần được thiết kế thế nào?
