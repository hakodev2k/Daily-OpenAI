# After (starter đã sửa)
- smoke: 202, exit 0.
- reproduce: exit 1 vì lỗi gốc không còn (đây là kết quả đúng sau fix).
- verify: PASS, exit 0.
- 503→202: đúng 2 partner calls, body JSON không rỗng, ISBN đúng, path /api/transfers, cùng Idempotency-Key.
- 400: 1 call, trả 400 không retry.
- 503→503: 2 calls, trả 503; không vượt retry budget.
- Không phát sinh network request thật hoặc phụ thuộc thời gian.
