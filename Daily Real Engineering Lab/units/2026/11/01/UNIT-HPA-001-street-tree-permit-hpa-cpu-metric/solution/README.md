# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce và tự sửa. YAML tham khảo: [deployment.yaml](deployment.yaml). `scripts/verify.ps1` chỉ kiểm tra starter.

## 1. Symptoms

Hai Pod có CPU usage, p95 tăng nhưng HPA `cpu: <unknown>/70%` và không tính được replica target.

## 2. Evidence

API có CPU request 250m. Audit agent có CPU limit 200m nhưng không có CPU request. HPA event chỉ rõ audit agent. metrics-server vẫn đo usage cho cả hai.

## 3. Root cause

HPA CPU Resource utilization cần CPU requests làm mẫu số. Khi một container trong Pod thiếu CPU request, CPU utilization của Pod không xác định; metric không được tính. `limits.cpu` không thay thế `requests.cpu`.

## 4. Why the fix works

Bổ sung `resources.requests.cpu: "50m"` và `requests.memory: "64Mi"` cho audit agent trong Deployment tham khảo. 50m là sizing của fixture, không phải giá trị chuẩn cho mọi cluster. Khi mọi container có request, HPA có thể tính tỷ lệ usage/request.

## 5. How to verify

Chỉnh `starter/deployment.yaml`, chạy `pwsh -File ./scripts/verify.ps1`. Total usage = 220+25+235+30 = 510m; total requests = 2×(250+50) = 600m; utilization = 85%. Desired sơ bộ = ceil(2×85/70) = 3. Kiểm tra `METRIC_READY`, `DESIRED_REPLICAS=3`, `VERIFY_PASS`.

## 6. Alternative fixes

- HPA ContainerResource metric nhắm riêng `permit-api` nếu version và chính sách hỗ trợ; phải xét container rename/missing.
- Custom/external metrics theo queue depth hoặc RPS nếu CPU không đại diện tải; cần metrics adapter đáng tin cậy.
- Rightsizing/VPA hỗ trợ capacity planning nhưng không thay thế sửa tín hiệu điều khiển hiện tại.

## 7. Wrong or misleading fixes

- Tăng maxReplicas không sửa metric unknown.
- Tăng CPU limit không tạo CPU request.
- Hạ target hoặc request cực thấp để ép scale làm sai tín hiệu và tăng rủi ro scheduling.
- Scale thủ công che lỗi; tắt audit agent vi phạm yêu cầu kiểm toán.

## 8. Production implications

Kiểm tra requests cho mọi container, metrics-server, HPA conditions/events, rollout, node allocatable, scheduling, readiness và downstream saturation. Canary, quan sát p95 và Pod Ready, chuẩn bị rollback. Local PASS không phải production PASS.

## 9. Trade-offs

Request cao giảm utilization tương đối nhưng giúp reservation/scheduling; request thấp tăng replica count và có thể tăng cost. HPA thực tế có tolerance, stabilization, readiness và missing-metric adjustments, không có trong mô hình local.

## 10. What a Senior engineer should notice

Phân biệt usage, utilization, request và limit. Đọc controller events trước khi đổi scaling knobs; sửa tín hiệu điều khiển trước, rồi kiểm chứng capacity thực tế.

## Tài liệu chính thức

- [Kubernetes Horizontal Pod Autoscaling](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)
- [Resource Management for Pods and Containers](https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/)
