# Hint 3 — Khái niệm

Kiểm tra hiệu lực tại request boundary: tồn tại, nowUtc < ExpiresAtUtc, và RevokedAtUtc chưa có hiệu lực. Không dùng thời điểm xóa document làm nguồn chân lý cho authorization.
