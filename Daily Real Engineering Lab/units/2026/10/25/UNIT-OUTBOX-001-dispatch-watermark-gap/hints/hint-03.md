# Hint 03 — Invariant và failure boundary

Với một dispatcher và yêu cầu phát theo thứ tự, checkpoint chỉ được mô tả **prefix liên tục đã acknowledge**. Xem điều gì xảy ra khi publish thất bại ở giữa batch: việc xử lý các record sau đó có còn bảo toàn invariant này không?
