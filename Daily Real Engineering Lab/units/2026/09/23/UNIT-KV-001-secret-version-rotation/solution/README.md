# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms
Sau configuration rotation, process chạy lâu tiếp tục nhận 401; process restart thì recover.

## Evidence
Network path vẫn hoạt động. Outcome thay đổi theo process restart và theo resource version mà process đang tham chiếu.

## Root cause
Application giữ một reference gắn với version cũ trong suốt process lifetime. Tạo version mới trong Key Vault không tự thay đổi reference đã được pin hoặc snapshot đã load trước đó.

## Why the fix works
Dùng reference theo logical secret name thay vì pin version khi yêu cầu nghiệp vụ là luôn theo version hiện hành, đồng thời thiết kế refresh lifecycle rõ ràng để long-running process có thể quan sát thay đổi mà không cần restart thủ công.

## How to verify
Trong môi trường kiểm thử: start hai instance, rotate sang một test version mới, không restart instance, chờ qua refresh interval đã định nghĩa và xác nhận cả hai chuyển sang version hiện hành. Kiểm tra failure behavior khi refresh tạm thời thất bại.

## Alternatives
Version pinning vẫn hợp lệ khi deployment phải cố định immutable configuration; khi đó rotation cần đi kèm rollout có chủ đích. Một configuration provider có reload/refresh cũng hợp lệ nếu semantics và polling cost phù hợp.

## Wrong / misleading fixes
Restart thủ công chỉ che lifecycle flaw nếu requirement là rotation không downtime. Tăng retry không giúp khi request lặp lại cùng stale configuration. Scale-out có thể tạo thêm instance với trạng thái khác nhau và làm incident khó hiểu hơn.

## Production implications
Cần định nghĩa refresh interval, observability cho version đang dùng, behavior khi Key Vault unavailable, rollback, và giới hạn blast radius. Không log secret value.

## What a Senior engineer should notice
Secret rotation là lifecycle contract giữa secret store, application configuration và downstream acceptance window; không chỉ là thao tác tạo một version mới.