# Reference Solution — chỉ đọc sau khi thử fix

## Symptoms
8 producer acknowledgments nhưng consumer chỉ nhận 3 sequence cuối.

## Evidence
Với capacity=3, producer ghi hết trước khi reader chạy; log ghi `accepted=True` cho từng mẫu nhưng audit chỉ thấy `6,7,8`.

## Root cause
`BoundedChannelFullMode.DropOldest` chủ động loại bỏ dữ liệu chưa đọc khi đầy; `TryWrite` thành công không đồng nghĩa audit được lưu bền vững. Đây là hợp đồng overflow, không phải lỗi scheduler.

## Why the fix works
Chuyển sang `BoundedChannelFullMode.Wait`, chạy consumer song song và dùng `await WriteAsync` để producer chờ khi đầy; complete writer và await consumer trước khi công bố audit hoàn tất. Xem [Program.cs](Program.cs).

## How to verify
Sửa `starter/Program.cs` theo cách riêng rồi chạy `./verify.ps1`. Sau đó so sánh reference với `dotnet run --project solution/AuditLab.csproj -- --verify`. Cả hai phải cho 8 sequence.

## Alternative fixes
Có thể dùng unbounded channel nếu dữ liệu hữu hạn và giới hạn bộ nhớ được kiểm soát; persistent queue/outbox nếu cần survive process crash. Với audit pháp lý, channel in-memory không bảo đảm durability.

## Wrong or misleading fixes
Tăng capacity lên 8 chỉ che burst hiện tại, vẫn mất dữ liệu khi burst lớn hơn. Thêm `Task.Delay` trước ghi không chứng minh losslessness. Kiểm tra mỗi `TryWrite` không phát hiện evictions khi drop mode.

## Production implications
Xác định rõ acknowledgment semantics: accepted into memory khác persisted/committed. Thêm metrics backlog, dropped events (nếu dùng lossy mode), shutdown drain, cancellation, restart recovery, và test overload.

## Trade-offs
Wait mode giữ dữ liệu nhưng tăng producer latency và lan truyền backpressure. Unbounded mode tránh chặn tạm thời nhưng rủi ro OOM. Durable broker/outbox tốn vận hành nhưng cần khi yêu cầu audit bền vững.

## What a Senior engineer should notice
Không thể hứa exactly-once durable audit bằng channel in-memory. Phải chốt SLA về loss, crash, ordering và replay trước khi chọn topology.
