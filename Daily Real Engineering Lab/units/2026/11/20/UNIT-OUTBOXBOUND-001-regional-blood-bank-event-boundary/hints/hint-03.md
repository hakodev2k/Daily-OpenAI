# Hint 3 — Cơ chế
Xem xét transactional outbox và consumer inbox/idempotency key. Chỉ dựa vào cờ `Sent` ở publisher có đủ để ngăn side effect lặp ở consumer khi ACK bị mất không?
