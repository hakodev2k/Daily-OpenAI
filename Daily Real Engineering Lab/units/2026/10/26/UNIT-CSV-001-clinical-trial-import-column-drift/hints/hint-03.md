# Hint 03 — Hướng triển khai

Cần parser có trạng thái **inside quoted field / outside quoted field**, xử lý dấu nháy kép escape và CRLF. Chỉ kết thúc record ở newline khi không nằm trong quoted field; từ chối dấu nháy chưa đóng.
