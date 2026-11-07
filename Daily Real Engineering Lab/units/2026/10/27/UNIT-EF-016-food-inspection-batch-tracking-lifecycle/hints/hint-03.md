# Hint 03 — Exception safety

Nếu một method tạm thời thay đổi `AutoDetectChangesEnabled`, điều gì phải được bảo toàn khi method thành công, ném exception, hoặc caller vốn đã đặt cấu hình khác mặc định? Nghĩ đến `try/finally` và original value.
