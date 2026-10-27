# Incident evidence (synthetic)

| Time | Source | Observation |
|---|---|---|
| 09:14:01 | Partner gateway | Manifest VN-604 / CHEM-77 submitted; partner UI says inspection required |
| 09:14:01 | Ingress | HTTP parse completed; no exception |
| 09:14:01 | Decision service | Manifest VN-604 / CHEM-77 => inspection=false |
| 09:16:20 | Operations | Re-submission from partner dashboard => inspection=true |
| 09:20:00 | SRE | No deployment, DB failover or cache outage during interval |

Đây là dữ liệu mô phỏng, không phải telemetry production. Hãy thu raw *synthetic* request trong `starter/Program.cs` và đối chiếu với DTO, không suy luận từ HTTP status đơn thuần. Không cần cloud hay log nhạy cảm.
