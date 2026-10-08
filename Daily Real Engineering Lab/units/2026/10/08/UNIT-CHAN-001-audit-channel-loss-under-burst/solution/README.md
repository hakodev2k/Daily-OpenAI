# Reference Solution — SPOILER

## Symptoms
Producer hoàn tất 12 lần publish; consumer chỉ nhận 4 sự kiện cuối (9..12). Không có exception.

## Evidence
`reproduce.ps1` cho thấy `published=12; observed=4; sequences=[9,10,11,12]`. `verify.ps1` trên starter ban đầu cho thấy producer không chịu áp lực khi consumer bị giữ lại và kết quả thiếu dữ liệu.

## Root cause
`BoundedChannelFullMode.DropOldest` là overflow policy mất dữ liệu có chủ đích: khi channel đầy, phần tử cũ nhất bị loại để nhận phần tử mới. `WriteAsync` hoàn tất không có nghĩa mọi event trước đó vẫn được giữ. Đây không phải race của consumer.

## Why the fix works
Giữ `capacity=4` nhưng chuyển sang `BoundedChannelFullMode.Wait`. Khi đầy, `PublishAsync` đợi consumer lấy bớt phần tử. `Program.cs` khởi chạy producer và consumer đồng thời sau khi mở gate; do đó không deadlock. `Complete()` báo không còn ghi mới; `ReadAllAsync` drain các sự kiện còn lại.

## How to verify
1. Chỉ thay `starter/AuditPipeline.cs` theo `solution/AuditPipeline.cs`.
2. Chạy `./scripts/verify.ps1`.
3. Cần thấy `boundedPressureObserved=True`, `completeOrdered=True`, `VERIFY PASSED`.
4. Không dùng `reproduce.ps1` sau khi sửa: script đó chỉ xác nhận trạng thái lỗi ban đầu.

## Alternatives
- `BoundedChannelFullMode.Wait` + timeout/cancellation rõ ràng: phù hợp xử lý trong tiến trình khi có thể chặn producer.
- Trả 429/503 hoặc admission failure ở API boundary nếu không thể chờ quá lâu; caller phải biết event chưa được nhận.
- Transactional outbox / durable broker: cần khi sự kiện audit không được phép mất ngay cả khi process crash; phức tạp và có chi phí vận hành.

## Wrong / tempting fixes
- Tăng capacity từ 4 lên 4000 chỉ trì hoãn sự cố khi burst vượt mức.
- Dùng unbounded channel khiến memory tăng không giới hạn khi consumer chậm; cũng không qua backpressure test.
- Bọc `PublishAsync` trong retry không giúp khi overflow được xem là thành công.
- Tăng số consumer không bảo đảm không mất dữ liệu và còn phải kiểm soát ordering.

## Production implications
Hợp đồng acceptance phải phân biệt “accepted in memory” và “durably persisted”. `Wait` không cung cấp crash durability; shutdown phải ngừng admission và drain có deadline. Theo dõi queue depth, enqueue wait time, dropped count (nếu drop được phép) và lag. Nếu dữ liệu có yêu cầu audit nghiêm ngặt, cân nhắc durable outbox.

## Trade-offs
Backpressure giữ correctness nhưng tăng latency và có thể làm API timeout khi downstream quá chậm. Capacity nhỏ dễ kiểm thử nhưng không tối ưu cho mọi workload. Lựa chọn cần dựa trên throughput, SLO, yêu cầu losslessness và failure budget.

## Senior engineer should notice
API `WriteAsync` thành công không tự động chứng minh end-to-end delivery. Cần nêu rõ acceptance semantics, lifecycle ownership, memory bounds, crash boundary và quan sát dữ liệu bị loại.

## References
- Microsoft Learn — Channels: https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
- Microsoft Learn — BoundedChannelFullMode: https://learn.microsoft.com/en-us/dotnet/api/system.threading.channels.boundedchannelfullmode
