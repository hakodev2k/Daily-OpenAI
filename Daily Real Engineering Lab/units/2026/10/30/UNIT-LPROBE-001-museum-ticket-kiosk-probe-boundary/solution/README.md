# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms

Khi partner scoring không khả dụng, /health/live và /health/ready đều trả 503. Kubernetes có thể coi container là hỏng và restart, dù process vẫn trả lời HTTP.

## Evidence

Chạy ./reproduce.ps1: normal 200/200, partner-down 503/503, recovered 200/200. Process không chết; HTTP vẫn trả response. Cần kiểm tra hợp đồng health checks trước khi kết luận crash hay network fault.

## Root cause

Endpoint /health/live chạy tất cả registered checks (Predicate = _ => true), bao gồm partner-scoring. Dependency unhealthy khiến aggregate health check thành 503, làm liveness thất bại. Dependency outage không có nghĩa process cần restart.

## Reference source

[ProbeHostFactory.cs](ProbeHostFactory.cs) là bản nguồn đầy đủ. Khác biệt cốt lõi: liveness chỉ chạy health check có tag live. Readiness tiếp tục chạy process và dependency check để ngừng nhận traffic khi đối tác không phục vụ được.

## Why the fix works

Liveness trả lời process còn hoạt động không; readiness trả lời instance có thể nhận traffic không. Lọc check theo tag tách failure domain. Khi partner down, liveness 200 và readiness 503 là trạng thái có chủ đích.

## How to verify

Tự sửa starter/ProbeHostFactory.cs rồi chạy ./verify.ps1. Contract nằm ở contracts/, khởi động chính project starter, không tham chiếu solution. Phải pass normal, partner-down và recovered. Reproduce sẽ không còn pass sau sửa vì chỉ dùng để xác nhận lỗi ban đầu.

## Alternative fixes

- Có thể dùng liveness check rỗng nhưng phải bảo đảm endpoint vẫn phản ánh process responsiveness.
- Có thể dùng startup probe riêng cho warm-up dài; startup probe không thay thế phân loại lỗi downstream.
- Nếu nghiệp vụ cho phép degraded/offline mode an toàn, readiness policy có thể khác nhưng phải được chứng minh bằng hợp đồng nghiệp vụ.

## Wrong / tempting fixes

- Tăng failureThreshold của liveness chỉ trì hoãn restart, không tách failure domain.
- Tắt readiness để luôn nhận traffic có thể cho phép xác thực vé thiếu kiểm soát.
- Scale pod hoặc restart partner ngay có thể khuếch đại tải lên hệ thống đang lỗi.

## Production implications

Kiểm tra livenessProbe và readinessProbe trỏ đúng route, quan sát từng check, đặt timeout/threshold phù hợp. Liveness phải phát hiện process hỏng thực sự nhưng không bị kéo xuống bởi transient downstream outage.

## Trade-offs

Tách probe giảm restart nhưng không phục hồi đối tác. Readiness 503 trên mọi pod có thể giảm capacity toàn deployment; cần đánh giá fallback, queue và yêu cầu nghiệp vụ. Không trả readiness 200 khi không thể xử lý an toàn.

## Senior engineer should notice

Phân biệt availability với correctness, fault isolation, shared dependency blast radius và chính sách access vào health endpoints. Không dùng probe làm thay thế cho telemetry và alerting.

## References

[Microsoft ASP.NET Core health checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) · [Kubernetes probes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/).
