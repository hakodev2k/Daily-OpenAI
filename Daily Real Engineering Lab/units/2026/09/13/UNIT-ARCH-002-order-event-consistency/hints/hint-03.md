# Hint 3

Một hướng phổ biến là lưu business change và một outbox record trong cùng SQL transaction, rồi có publisher riêng đọc và publish các record chưa gửi. Khi làm vậy, hãy nghĩ tiếp về duplicate publish, idempotent consumer, retention và monitoring backlog.
