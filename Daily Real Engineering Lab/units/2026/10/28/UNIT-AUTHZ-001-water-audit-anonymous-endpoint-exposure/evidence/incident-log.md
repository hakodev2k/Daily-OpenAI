# Incident evidence (synthetic)

| Time | Request | Auth context | HTTP | Observation |
|---|---|---|---|---|
| 08:11 | GET /api/audits/A-204 | none | 200 | Internal audit record returned |
| 08:12 | GET /api/audits/A-204 | demo auditor | 200 | Expected authorized read |
| 08:13 | GET /api/audits/export | none | 200 | Export metadata returned |
| 08:14 | GET /health | none | 200 | Public liveness check succeeds |
| 08:15 | dashboard static assets | none | 304 | Unrelated cache observation |

Các dòng trên là dữ liệu mô phỏng, không phải production telemetry thật. Dùng script để xác nhận lại HTTP statuses trước khi kết luận. Không có real customer data.
