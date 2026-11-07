# Hint 03

Một hướng thiết kế là ràng buộc idempotency entry với identity ổn định của request gốc. Khi cùng key xuất hiện với identity khác, hệ thống nên xử lý như contract conflict thay vì trả lại kết quả cũ.
