# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Khi database chuyển sang unavailable, service đồng thời trở thành not-ready và not-live. Simulator vì vậy tăng restart count trong đúng khoảng thời gian dependency đang lỗi.

## Evidence

Output cho thấy `db=DOWN`, `ready=False`, `live=False` và `restarts` tăng. Không có evidence nào cho thấy process bị deadlock, crash hoặc mất khả năng tự phục hồi.

## Root cause

Liveness decision phụ thuộc trực tiếp vào external database health. Kubernetes có thể dùng liveness failure để restart container, nên một transient dependency outage bị biến thành restart loop.

## Why the fix works

Tách process liveness khỏi dependency readiness. Database outage vẫn làm readiness fail để instance không nhận traffic mới, nhưng process vẫn được coi là alive nếu bản thân nó còn có thể chạy và chờ dependency phục hồi.

## How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả mong đợi: các tick `db=DOWN` có `ready=False`, nhưng `restartCount=0` ở cuối.

## Alternative fixes

- Dùng ASP.NET Core Health Checks với tag riêng cho `/health/live` và `/health/ready`.
- Giữ liveness cực tối giản, chỉ phản ánh process/runtime state; readiness có thể kiểm tra critical dependencies.
- Với startup chậm, cân nhắc `startupProbe` thay vì nới liveness một cách tùy tiện.

## Wrong or misleading fixes

- Tăng `failureThreshold` rất lớn chỉ làm restart loop chậm hơn nếu contract liveness vẫn sai.
- Tăng replica count không sửa probe semantics và có thể khiến nhiều pod restart đồng thời.
- Bỏ toàn bộ dependency check khỏi mọi health endpoint có thể khiến instance tiếp tục nhận traffic dù không phục vụ request được.

## Production implications

Probe contract là một phần của failure-control loop. Một signal sai có thể khuếch đại dependency incident thành application outage, tăng cold-start load và gây thundering herd khi nhiều pod cùng restart.

## Trade-offs

Readiness kiểm tra dependency quá sâu hoặc quá thường xuyên cũng có chi phí và có thể tạo false negative. Chỉ nên kiểm tra những dependency thực sự quyết định khả năng phục vụ traffic của instance, với timeout phù hợp.

## What a Senior engineer should notice

Health checks không chỉ là endpoint HTTP. Chúng điều khiển orchestration behavior. Cần thiết kế signal theo action mà orchestrator sẽ thực hiện khi signal fail, không chỉ theo câu hỏi “dependency có khỏe không?”.
