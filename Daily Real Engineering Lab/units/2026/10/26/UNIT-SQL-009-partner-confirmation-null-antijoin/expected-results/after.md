# After
- `verify.ps1` kiểm tra **chính xác** `E-102` và `E-103` thuộc tenant A.
- Không đưa hợp đồng SUSPENDED, hợp đồng đã xác nhận, batch cũ hoặc tenant B vào danh sách.
- Kiểm tra cả tập kết quả và tổng số dòng, không chỉ kiểm tra "có ít nhất một dòng".
- Nếu verify thất bại: chạy lại reset, kiểm tra file đã sửa đúng `starter/query.sql` và xem lỗi từ assertion SQL.
