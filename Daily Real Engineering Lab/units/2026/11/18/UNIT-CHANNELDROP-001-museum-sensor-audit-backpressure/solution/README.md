# Reference Solution — Spoiler

**Root cause:** `BoundedChannelFullMode.DropOldest` loại bỏ ID 101 khi buffer capacity=2 đã đầy. `WriteAsync` thành công không có nghĩa event đã được persist. Starter viết cả ba trước khi đọc nên luôn mất ID đầu.

**Fix:** `FullMode=Wait`, khởi động consumer đồng thời với producer, `TryComplete` writer rồi await consumer drain. Mở `solution/Program.cs` để xem code.

**Verify learner:** chỉ sửa `starter/Program.cs`, sau đó chạy `scripts/verify.ps1`. Không copy output từ solution sang verify.

**Wrong fixes:** tăng capacity lên 3 chỉ che giấu lỗi khi burst lớn hơn; thêm retry sau `WriteAsync` không phát hiện drop; bỏ `TryComplete` có thể làm consumer chờ vô hạn.

**Trade-offs:** Wait tạo backpressure và tăng latency producer; cần cancellation/timeout, dead-letter hoặc durable broker nếu không thể mất audit sau process crash. Channel trong bộ nhớ không bảo đảm durability.

**Reference check:** `dotnet run --project solution/MuseumAudit.csproj` phải in `Accepted=3 Persisted=3` và `Ids=101,102,103`.
