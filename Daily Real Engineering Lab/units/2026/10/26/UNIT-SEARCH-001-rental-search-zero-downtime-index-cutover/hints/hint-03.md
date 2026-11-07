# Hint 03 — Chuyển hướng và rollback

Nghiên cứu cơ chế versioned index, CDC có entity version/tombstone và một read alias có thể đổi đích. Một lần đổi alias chỉ an toàn khi đã xác minh correctness, freshness, query contract và cách giữ index cũ đủ mới để rollback. Đừng xem số lượng document bằng nhau là đủ.
