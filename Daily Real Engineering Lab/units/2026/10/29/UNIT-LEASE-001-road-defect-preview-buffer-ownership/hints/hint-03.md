# Hint 03 — Khái niệm
Một buffer được thuê từ ArrayPool<byte> có thể được tái sử dụng ngay sau Return. Nếu dữ liệu cần tồn tại lâu hơn lease, phải có một owner phù hợp hoặc một bản sao riêng trước khi trả lease. So sánh copy-out với lease-based ownership và chi phí phân bổ.
