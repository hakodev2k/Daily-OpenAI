# Synthetic incident evidence — local policy exercise

Dữ liệu dưới đây mô phỏng kết quả đã quan sát tại một cluster Kubernetes; **không** được tạo bằng cách chạy cluster trong lab.

| Time (ICT) | Signal | Value |
|---|---|---|
| 16:42 | Deployment desired / Ready | 3 / 3 |
| 16:43 | PDB `minAvailable` | `80%` |
| 16:44 | Internal maintenance planner | `predictedDisruptionsAllowed=1` |
| 16:45 | `kubectl get pdb ambulance-dispatch` | `ALLOWED DISRUPTIONS: 0` |
| 16:46 | Node drain | Eviction refused: would violate disruption budget |
| 16:47 | API p95 latency | 82 ms, normal; CPU 19% |
| 16:49 | Scale-up trial, 5 / 5 Ready | PDB `ALLOWED DISRUPTIONS: 1` |

Current PDB policy (illustrative):
```yaml
apiVersion: policy/v1
kind: PodDisruptionBudget
metadata:
  name: ambulance-dispatch
spec:
  minAvailable: "80%"
  selector:
    matchLabels:
      app: ambulance-dispatch
```

**Giới hạn evidence:** không có log lỗi ứng dụng, không có Pod crash, không có lỗi network. Việc drain bị chặn là một voluntary eviction decision, không chứng minh ứng dụng đang lỗi.
