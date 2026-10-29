# Hint 3 — Hợp đồng giao nhận
Trong mô hình at-least-once, acknowledgment không đồng nghĩa với việc đã **thử** xử lý. Cần phân biệt thành công, cancellation và kết quả upload chưa rõ. Nếu retry sau một side effect, key artifact phải đủ ổn định để chống ghi trùng.
