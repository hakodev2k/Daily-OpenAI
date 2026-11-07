# Senior Challenge
1. Thêm HTTP 413 khi payload vượt 1 MiB và kiểm tra cleanup.
2. Thêm integration test với Kestrel hoặc WebApplicationFactory và so sánh forward-only với seekable stream.
3. Thiết kế audit không ghi PII, token hoặc raw body.
4. Kiểm tra cancellation, concurrent requests, temporary-file pressure và streaming upload.
5. So sánh buffering với parse-once và streaming tee về cost, latency và operational risk.
