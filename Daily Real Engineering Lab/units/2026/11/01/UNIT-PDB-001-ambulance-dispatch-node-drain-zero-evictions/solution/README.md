# Reference Solution — SPOILER

**Chỉ xem sau khi reproduce, ghi hypothesis và thử fix.**

## 1. Symptoms
Planner dự báo drain một Pod trong nhóm ba replica, trong khi Kubernetes báo `disruptionsAllowed=0`; node maintenance không tiến hành được dù cả ba Pod Ready.

## 2. Evidence
PDB `minAvailable: "80%"`; Deployment 3/3 Ready; planner=1; Kubernetes=0. Khi tăng lên 5/5 Ready, Kubernetes cho phép 1. CPU và p95 bình thường, không có Pod crash: lỗi nằm ở maintenance policy modeling, không phải request path.

## 3. Root cause
Phần trăm `minAvailable` được Kubernetes làm tròn **lên** để xác định `desiredHealthy`. Starter dùng `Math.Floor` nên với 3 Pod và 80%, nó lấy 2 thay vì 3. Tương tự 2 Pod và 51%: Kubernetes yêu cầu cả 2 healthy. Đây là sai lệch policy contract, không phải Kubernetes bị lỗi.

## 4. Why the fix works
[Reference PdbCalculator.cs](PdbCalculator.cs) dùng `Math.Ceiling(expectedPods * percent / 100.0)` và `Math.Max(0, healthyPods - desiredHealthy)`. Giữ input validation; không làm tròn ở cuối phép trừ hoặc hard-code replica count.

## 5. How to verify
Sửa `starter/PdbCalculator.cs`, chạy `./scripts/verify.ps1`. Script chạy code learner-editable, không chạy reference. Exit 0, `CONTRACT_MISMATCHES=0` là điều kiện cần. Với cluster thực, kiểm tra `kubectl get pdb`, `kubectl describe pdb`, Deployment selector và số Pod healthy trước drain.

## 6. Alternative decisions
- **Tạm tăng replica từ 3 lên 5**, chờ cả năm Ready và đủ node capacity, sau đó drain một Pod mỗi lần; giữ `minAvailable: 80%`. Chi phí và capacity tăng, nhưng mục tiêu 80% được giữ.
- **Đặt PDB `maxUnavailable: 1`** cho workload có nhiều replica, kết hợp readiness và capacity gate. Cho phép một voluntary eviction, nhưng không tự bảo đảm 80% ở ba replica; khi đã có một Pod unready, disruption allowance có thể về 0. Phải phê duyệt trade-off với SLO.
- Nếu không có spare capacity, hoãn maintenance hoặc mở rộng cluster trước; không xóa PDB như một thao tác mặc định.

## 7. Wrong / tempting fixes
- `kubectl drain --force` không phải biện pháp để bypass PDB của API critical; đừng dùng thao tác phá availability để sửa một dự báo sai.
- Tăng timeout drain không thay đổi desiredHealthy.
- Chỉ kiểm tra `kubectl get pods` và thấy 3/3 Ready không suy ra được disruption budget.
- Đổi `minAvailable` thành 66% mà không đánh giá SLO có thể hạ ngưỡng availability ngoài ý muốn.

## 8. Production implications
PDB chỉ điều phối **voluntary disruptions** qua eviction API; nó không ngăn crash, node failure hay mọi cách xóa Pod. Kiểm tra selector, replica count, currentHealthy, unhealthy pod eviction policy, rollout surge và capacity thực. Đo drain completion, failed evictions, pending pods, p95 và request errors; có rollback/abort criteria.

## 9. Trade-offs
Scale-up tăng chi phí nhưng giữ mức availability; `maxUnavailable` rõ hơn về số Pod có thể mất nhưng không biểu đạt phần trăm SLO. Policy nào cũng phải được kiểm tra tại các mức replica nhỏ và khi degraded.

## 10. Senior engineering insight
Không suy luận Kubernetes availability từ dashboard tự viết nếu chưa đối chiếu semantics của controller. Viết golden contract tests ở các biên số học; quyết định bảo trì phải xét cả desired capacity, health và thời gian khôi phục.

## References
- [Kubernetes — Configure a PodDisruptionBudget](https://kubernetes.io/docs/tasks/run-application/configure-pdb/)
- [Kubernetes — Disruptions](https://kubernetes.io/docs/concepts/workloads/pods/disruptions/)
