# Hint 01 — Phân biệt hai nguồn lỗi

So sánh số lần provider thực sự nhận request với số lần caller nhận exception. Nếu upstream đã phục hồi nhưng không có request mới, hãy kiểm tra trạng thái tồn tại bên trong process.
