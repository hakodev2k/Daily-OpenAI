# Hint 3 — Cơ chế
Đọc tài liệu về `ExecuteUpdateAsync` và ChangeTracker. Sau khi một lệnh cập nhật trực tiếp database hoàn tất, hãy cân nhắc đồng bộ lại entity đang tracked **trước khi** thực hiện phép tính phụ thuộc vào giá trị đó. Sau đó kiểm tra race giữa các request riêng biệt.
