# Reference Solution — spoiler

## Root cause
Projection ghi theo arrival order. Event sequence 2 đến sau sequence 3 khiến trạng thái Refund bị ghi đè bởi Paid.

## Fix
Chỉ nhận sequence lớn hơn sequence đã lưu, bỏ qua event cũ và duplicate. Xem `Program.cs`.

## Verification
`dotnet run --project solution/TicketReplay.csproj -- --verify` phải PASS. `verify.ps1` luôn kiểm tra `starter/` của người học.

## Trade-offs
Guard in-memory chỉ là mô phỏng. Production cần conditional update / transaction trong database để tránh race giữa các instance. Sequence phải cấp nhất quán theo aggregate. Nếu transition không thể bỏ qua, gap phải buffer/replay hoặc fetch authoritative snapshot. Partition theo ticket giảm reorder nhưng không thay thế idempotency. Theo dõi stale drops, sequence gaps và lag.

## Wrong fixes
- Sleep/Delay: phụ thuộc timing.
- Dedupe event ID đơn thuần: vẫn có event cũ khác ID.
- Sort trong một batch: không ngăn event đến ở batch sau.

## Reference
https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications
