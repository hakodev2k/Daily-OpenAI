# Incident snapshot — dữ liệu mô phỏng

| Tín hiệu | Quan sát |
|---|---|
| API p95 | 310 ms → 2.4 s |
| Ready replicas | 2/2, không tăng trong 8 phút |
| HPA | `cpu: <unknown>/70%`, desiredReplicas = 2 |
| HPA event | `FailedGetResourceMetric: missing request for cpu in container audit-agent of Pod permit-7d6c-a` |
| metrics-server | Có CPU usage cho cả hai containers |
| API log | `permit.created` thành công nhưng response chậm; không có OOMKilled |
| Audit log | Một số lần flush chậm; chưa chứng minh lỗi dữ liệu |

CPU snapshot:

| Pod | permit-api | audit-agent |
|---|---:|---:|
| permit-7d6c-a | 220m | 25m |
| permit-7d6c-b | 235m | 30m |

Dashboard hiển thị `CPU limit=200m` cho audit agent. Limit và request không cùng ý nghĩa. Kiểm tra cấu hình trước khi chọn giả thuyết.

Trên cluster thật có thể xem `kubectl describe hpa tree-permit-api`, `kubectl get deployment tree-permit-api -o yaml`, `kubectl top pod --containers`.
