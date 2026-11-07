# Hint 3 — Hợp đồng diff

Hãy dùng BaseSha đầu vào làm điểm so sánh rõ ràng. Trước khi diff, cần bảo đảm Git checkout có object của commit đó. Khi không thể xác minh base, gate phải fail closed thay vì trả empty change set.
