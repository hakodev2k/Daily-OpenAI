# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce và thử sửa starter.** Chạy reference độc lập:

\`\`\`powershell
dotnet run --project solution/AuditLab.Reference.csproj -- verify
\`\`\`

## Symptoms

Publisher báo hoàn thành cả 5 sự kiện, nhưng consumer chỉ nhận sequence 4 và 5. Không có exception báo mất dữ liệu. Kiểm tra hủy producer cũng không thấy tác vụ chờ.

## Evidence

Harness cố tình khởi động publisher trước consumer. Với capacity 2, \`producerCompletedBeforeConsumer=True\` và \`received=2\`. Bài test không phụ thuộc timing của máy vì publisher ở chế độ lỗi hoàn thành đồng bộ trước khi consumer chạy.

## Root cause

\`BoundedChannelFullMode.DropOldest\` là **chính sách mất dữ liệu có chủ đích**: khi đầy, ghi mới được chấp nhận và phần tử cũ chưa đọc bị loại bỏ. \`WriteAsync\` thành công không đồng nghĩa từng event đã được consumer nhận. Đó là lựa chọn hợp lý cho telemetry có thể bỏ mẫu, nhưng không phù hợp với audit trail cần đủ mọi event.

## Why the fix works

Reference đổi \`FullMode\` sang \`BoundedChannelFullMode.Wait\`. Khi đầy, publisher phải chờ consumer tạo chỗ trống. Trong harness, producer bị chặn ở event thứ ba cho tới khi consumer chạy; mọi event được giữ đúng thứ tự. Khi không có consumer, \`CancellationToken\` hủy \`WriteAsync\` đang chờ thay vì để pipeline treo vô hạn.

## How to verify

\`./verify.ps1\` phải chạy trên **starter đã sửa**, xác nhận 5/5 event, thứ tự 1..5, producer không thể hoàn thành trước consumer, và hủy producer đang chờ hoạt động. Reference có project riêng chỉ để đối chiếu.

## Alternative fixes

- Persistent append-only log/outbox: cần nếu audit phải sống sót qua process crash; tăng độ phức tạp, storage và vận hành.
- \`Channel.CreateUnbounded\`: không mất event vì đầy nhưng có thể tăng bộ nhớ không giới hạn nếu consumer chậm.
- Reject-fast với lỗi rõ ràng: hợp lý khi hợp đồng cho phép caller retry, nhưng **không** thể báo success cho event bị bỏ.

## Wrong / tempting fixes

- Tăng capacity lên 10.000: chỉ trì hoãn mất dữ liệu khi burst lớn hơn; không thay đổi contract.
- Thêm \`Task.Delay\` trước khi publish: phụ thuộc scheduling, che giấu nguyên nhân.
- Bắt exception và retry: DropOldest không ném exception cho event bị loại.
- Chỉ tăng consumer parallelism: có thể cải thiện throughput nhưng không chứng minh lossless khi producer burst và consumer chưa sẵn sàng.

## Production implications

In-memory Channel không cung cấp durability, exactly-once, cross-replica ordering hay khả năng replay sau crash. Khi audit là dữ liệu pháp lý, phải xác định commit boundary: lưu durable trước khi trả success, có event ID/idempotency, theo dõi backlog, thời gian chờ, dead-letter, và reconciliation. Với nhiều producer, xem lại \`SingleWriter\`; với nhiều consumer, cần thiết kế lại ordering.

## Trade-offs

Wait tạo backpressure và tăng latency publisher khi consumer chậm; đổi lại không tự ý xóa event. Hệ thống phải định nghĩa timeout/cancellation, cách phản hồi upstream, và SLO cho consumer. Không nên dùng bộ nhớ vô hạn để tránh thiết kế capacity.

## What a Senior engineer should notice

Chọn policy queue là **quyết định nghiệp vụ về data loss**, không chỉ tối ưu performance. Test cần assert end-to-end delivery và behavior khi capacity đầy, không chỉ assert số lệnh \`WriteAsync\` hoàn thành. Tham khảo tài liệu chính thức: https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
