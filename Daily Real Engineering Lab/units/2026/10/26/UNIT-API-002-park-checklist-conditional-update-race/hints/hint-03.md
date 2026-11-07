# Hint 03 — Khái niệm

Điều kiện `If-Match` và việc ghi revision cần nằm trong cùng atomic compare-and-swap boundary. Với lưu trữ in-memory, có thể dùng một critical section ngắn; với SQL, cân nhắc conditional UPDATE và kiểm tra số hàng bị ảnh hưởng.
