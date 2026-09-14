# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Khi fake downstream chuyển sang unavailable, process ASP.NET Core vẫn chạy nhưng `/health/live` trả 503. Với Kubernetes liveness probe, failure kéo dài có thể làm container bị restart dù nguyên nhân nằm ngoài process.

## 2. Evidence

- Process không exit.
- CPU/memory không phải tín hiệu chính của incident.
- Dependency outage làm cả liveness và readiness cùng fail.
- Khi dependency phục hồi, process hiện tại có thể phục vụ lại mà không cần restart.

## 3. Root cause

Liveness contract bị coupling với availability của external dependency. Nó biến một lỗi readiness/dependency thành tín hiệu rằng process cần restart.

## 4. Why the fix works

Tách hai signal:

- **liveness** trả healthy khi process còn vận hành và có khả năng tự tiếp tục.
- **readiness** phản ánh dependency cần thiết để instance nhận traffic.

Khi dependency outage, orchestrator có thể loại pod khỏi traffic mà không phá hủy một process khỏe mạnh. Khi dependency phục hồi, readiness tự trở lại healthy.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Expected:

- dependency unhealthy → `/health/live` = 200
- dependency unhealthy → `/health/ready` = 503
- dependency healthy trở lại → `/health/ready` = 200

## 6. Alternative fixes

- Dùng ASP.NET Core Health Checks với tags riêng cho `live` và `ready`.
- Nếu process có internal deadlock detector hoặc unrecoverable self-check, signal đó có thể tham gia liveness.
- Startup probe có thể tách riêng cho ứng dụng khởi động chậm.

## 7. Wrong / Tempting Fixes

- **Tăng failureThreshold thật lớn:** có thể giảm restart nhưng vẫn giữ sai semantics.
- **Xóa mọi dependency check:** làm readiness mất khả năng bảo vệ traffic.
- **Restart nhanh hơn khi dependency lỗi:** thường khuếch đại outage và tạo thundering herd.
- **Scale out ngay:** tăng số process không sửa contract probe sai.

## 8. Production implications

Probe semantics là một phần của failure isolation. Một health signal sai có thể biến outage cục bộ thành outage toàn fleet. Cần quan sát restart count, readiness state, downstream availability và capacity còn lại cùng nhau.

## 9. Trade-offs

Readiness phụ thuộc downstream có thể loại nhiều pod khỏi traffic cùng lúc nếu dependency global bị outage. Trong một số hệ thống, degraded-mode hoặc fallback có thể tốt hơn việc fail readiness hoàn toàn. Quyết định phụ thuộc khả năng service phục vụ hữu ích khi dependency mất.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi endpoint health có trả 200 hay không; họ hỏi **mỗi signal điều khiển hành động nào của orchestrator** và liệu hành động đó có thực sự giúp phục hồi failure mode đang xảy ra hay không.
