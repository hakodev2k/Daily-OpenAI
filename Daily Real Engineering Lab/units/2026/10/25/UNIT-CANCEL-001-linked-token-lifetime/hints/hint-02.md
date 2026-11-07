# Hint 2 — Hai lifetime

Một method trả về `Task<T>` không nhất thiết chờ task hoàn thành. Vẽ timeline gồm: tạo tài nguyên liên kết, gọi scanner, method trả về, caller cancel, scanner kết thúc. Đánh dấu lúc tài nguyên được giải phóng.
