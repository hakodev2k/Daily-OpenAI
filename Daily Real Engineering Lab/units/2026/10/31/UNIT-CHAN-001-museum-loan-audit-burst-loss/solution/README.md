# Reference Solution — spoiler

## 1. Symptoms
Producer nhận 9 scan nhưng audit store chỉ có sequence 7–9; reconciliation báo thiếu 6 event, không có exception.

## 2. Evidence
TryWrite trả success cho event gửi vào buffer; sau burst, reader chỉ thấy 3 event cuối. Các event đầu biến mất trước callback persist. CPU thấp và heartbeat thành công không loại trừ mất dữ liệu.

## 3. Root cause
BoundedChannelFullMode.DropOldest với capacity 3 tự loại event cũ khi producer ghi nhanh hơn reader. TryWrite không phải cam kết durability. Producer đẩy 9 event trước khi consumer bắt đầu.

## 4. Why the fix works
Đổi sang BoundedChannelFullMode.Wait, dùng WriteAsync và cho consumer chạy song song. Khi đầy, producer đợi thay vì làm mất event. Consumer hoàn thành hoặc ném lỗi; nhánh catch đóng writer để unblock producer đang chờ. [Mã tham khảo](AuditPipeline.cs) có thể copy thay starter/AuditPipeline.cs.

## 5. How to verify
Chạy ./verify.ps1 sau khi sửa starter. Case 0, 1, 9, 40 phải giữ sequence. Callback persist ném sink-unavailable tại sequence 2 phải làm cả batch thất bại. ./reproduce.ps1 sau khi sửa không còn xác nhận baseline loss là đúng.

## 6. Alternative fixes
Unbounded channel tránh drop trong burst nhỏ nhưng có thể làm RAM tăng không giới hạn. Audit bắt buộc bền vững qua process crash cần transactional outbox, durable broker hoặc durable write trước ACK.

## 7. Wrong / tempting fixes
- Tăng capacity chỉ trì hoãn sự cố.
- DropNewest hay DropWrite vẫn mất audit.
- Đếm TryWrite success để báo durability là sai.
- Retry ở consumer không phục hồi event đã bị evict.
- Chỉ đổi sang Wait nhưng enqueue hết trước khi khởi động consumer có thể kẹt.

## 8. Production implications
Backpressure chuyển áp lực sang producer: cần deadline, cancellation, timeout budget, SLO và admission policy. ACK sau enqueue in-memory không đảm bảo đã commit hoặc fsync; không quảng cáo exactly-once durability khi chưa có durable transaction.

## 9. Trade-offs
Lossless bounded buffering bảo vệ RAM và dữ liệu khi process còn sống nhưng tăng latency khi sink chậm. Durable queue/outbox cải thiện recovery nhưng thêm I/O, vận hành và idempotency. Đo queue depth, enqueue wait time, consumer errors và persisted count.

## 10. Senior engineer should notice
Phải xác định contract của accepted, queued, persisted, acknowledged. Test burst, sink failure, shutdown/cancellation; tách correctness khỏi throughput.
