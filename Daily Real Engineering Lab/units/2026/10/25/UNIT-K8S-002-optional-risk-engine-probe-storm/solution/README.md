# Reference Solution — SPOILER

Chỉ mở sau khi reproduce và tự thử sửa starter/ProbePolicy.cs.

## Symptoms
Trong outage risk scoring, 2 restarts và 4 dropped requests xảy ra dù core session có thể degraded-mode.

## Evidence
Process và primary store healthy suốt 8 ticks. ServeSession đã có nhánh 200-fallback khi risk engine unavailable nhưng router loại instance khỏi traffic.

## Root cause
IsLive dùng tình trạng optional risk engine để quyết định restart; IsReady cũng yêu cầu engine. Optional outage vì vậy gây restart và routing outage không cần thiết.

## Why the fix works
Liveness chỉ kiểm tra khả năng process tiếp tục chạy. Readiness kiểm tra process và primary store vì heartbeat cần durable write. Optional risk scoring không quyết định hai trạng thái đó, nhưng được ghi nhận bằng degraded response và metric.

## Verification
Sau khi tự sửa, có thể đối chiếu solution/ProbePolicy.cs và chạy ./scripts/verify.ps1 trên starter. 14/14 checks PASS; demo restarts=0 dropped=0 fallback=4 personalized=4.

## Alternative fixes
Nếu compliance bắt buộc risk scoring trước khi nhận session, readiness strict có thể là lựa chọn đúng nhưng SLA phải thừa nhận outage. Nếu có durable queue thay primary store, readiness có thể thay đổi khi đã chứng minh replay correctness. Async risk enrichment giảm coupling khi nghiệp vụ cho phép.

## Wrong / tempting fixes
Tăng failureThreshold chỉ trì hoãn restart. Tăng replicas có thể nhân số instance cùng bị loại. Tắt mọi probe che process failure. Always-ready khi primary store mất có thể làm mất dữ liệu.

## Production implications
Tách /health/live và /health/ready theo contract; có startupProbe khi cold start; alert riêng cho degraded scoring; giới hạn timeout và retry của dependency; canary và fault injection ở staging.

## Trade-offs
Liveness quá strict tạo restart storm; quá permissive bỏ sót process bị kẹt. Readiness strict bảo vệ consistency nhưng giảm availability. Degraded scoring bảo toàn core SLA nhưng giảm độ chính xác cảnh báo, cần business approval và audit.

## Senior engineer insight
Định nghĩa criticality từ business invariants và SLO trước khi viết probe. Simulator không thay thế staging test trên Kubernetes thật.

## Reference code
[ProbePolicy.cs](ProbePolicy.cs) là bản sửa policy đầy đủ; không chỉnh simulator hoặc test oracle.
