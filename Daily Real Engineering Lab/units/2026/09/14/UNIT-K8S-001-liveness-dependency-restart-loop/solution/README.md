# Reference Solution

> Chỉ xem sau khi đã reproduce và tự thử fix.

## 1. Symptoms

Khi fake catalog dependency unavailable, endpoint được dùng làm liveness probe trả về failure. Orchestrator vì thế restart process dù bản thân process không bị hỏng.

## 2. Evidence

- `/health/live` chuyển sang non-success chỉ vì dependency ngoài process down.
- Restart process không thay đổi trạng thái dependency.
- Restart counter tăng nhưng outage gốc vẫn còn.

## 3. Root cause

Cùng một dependency health check đang tham gia cả liveness và readiness contract. Điều này đồng nhất hai câu hỏi khác nhau:

- process có còn sống và có khả năng tiếp tục chạy không?
- instance có nên nhận traffic lúc này không?

External dependency outage thường ảnh hưởng readiness, không nhất thiết có nghĩa process cần bị restart.

## 4. Why the fix works

Reference implementation gắn tag riêng cho health checks và lọc từng endpoint:

- `live`: chỉ kiểm tra process/self health.
- `ready`: kiểm tra self health và catalog dependency.

Khi catalog down, pod có thể bị loại khỏi traffic nhưng process không bị restart vô ích.

## 5. How to verify

Copy cách tổ chức health checks từ `solution/Program.cs` sang `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Expected:

- `/health/live` = 200
- `/health/ready` = non-200 khi catalog unavailable

## 6. Alternative fixes

- Dùng riêng startup probe nếu application có startup kéo dài.
- Readiness có thể kiểm tra dependency trực tiếp hoặc dựa trên local state/circuit state tùy chi phí và failure semantics.
- Với một số dependency optional, ngay cả readiness cũng không nên fail; cần dựa trên business capability thực tế.

## 7. Wrong or misleading fixes

- Tăng restart backoff chỉ làm giảm tần suất symptom, không sửa health contract.
- Tăng replica count có thể khuếch đại load lên dependency đang lỗi.
- Bỏ health probes hoàn toàn làm mất cơ chế traffic management và process recovery hợp lệ.
- Cho liveness luôn trả 200 mà không có bất kỳ self-health signal nào cũng có thể che process deadlock trong hệ thống thực tế.

## 8. Production implications

Sai health-probe semantics có thể biến outage cục bộ thành cascading failure: restart storm, cold-start load, mất cache nóng, connection churn và tăng áp lực lên dependency đang suy giảm.

## 9. Trade-offs

Readiness check càng sâu càng phát hiện nhiều dependency problem, nhưng cũng tăng coupling và có thể loại toàn bộ fleet khỏi traffic khi dependency chung bị down. Probe design cần phản ánh capability mà instance thực sự cung cấp.

## 10. What a Senior engineer should notice

Health endpoint là operational contract với orchestrator, không phải chỉ là một URL trả `Healthy/Unhealthy`. Senior engineer phải xác định rõ mỗi signal sẽ kích hoạt hành động gì: restart process, stop traffic, hay chỉ alert. Thiết kế signal sai nghĩa có thể làm control plane khuếch đại failure thay vì giúp recovery.
