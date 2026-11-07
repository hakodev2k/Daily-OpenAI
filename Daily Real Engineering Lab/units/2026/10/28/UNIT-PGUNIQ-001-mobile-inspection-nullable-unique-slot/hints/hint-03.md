# Hint 03 — Hướng sửa
PostgreSQL 15+ có một biến thể của UNIQUE cho phép xem NULL là không phân biệt nhau trong kiểm tra uniqueness. Áp dụng cho khóa lịch phù hợp; trước khi đổi constraint trên production, tìm và xử lý những bản ghi đã trùng.
