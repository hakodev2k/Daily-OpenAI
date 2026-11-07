# Senior Extension — Crash Boundary

Giả sử dịch vụ trả HTTP 202 ngay sau \`WriteAsync\`, sau đó process bị kill trước khi consumer ghi database. Thiết kế một contract có thể phục hồi: định nghĩa ACK boundary, durable append, event ID, retry/idempotency, thứ tự theo BatchId, consumer checkpoint, metrics backlog và shutdown draining. Đánh giá tại sao in-memory Wait chỉ giải quyết backpressure chứ không giải quyết durability.
