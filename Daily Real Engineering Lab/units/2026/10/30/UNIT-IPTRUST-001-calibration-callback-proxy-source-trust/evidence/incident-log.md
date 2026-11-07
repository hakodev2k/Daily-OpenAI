# Incident evidence (synthetic)
Dữ liệu được dựng xác định, không phải production log.

| Time | Remote TCP peer | X-Forwarded-For | Observed | Expected |
|---|---|---|---|---|
| 09:01:13 | 10.0.0.10 | 203.0.113.10 | ALLOW | ALLOW |
| 09:02:41 | 198.51.100.77 | 203.0.113.10 | ALLOW | DENY |
| 09:03:05 | 10.0.0.10 | 198.51.100.77 | DENY | DENY |
| 09:04:22 | 10.0.0.10 | 203.0.113.10, 198.51.100.77 | ALLOW | DENY |

Context: proxy managed `10.0.0.10`; allowed partner `203.0.113.10`; policy đúng một proxy hop. Một đợt triển khai chứng chỉ TLS cũng diễn ra cùng ngày nhưng không có bằng chứng liên hệ với quyết định allowlist. Hãy tách correlation khỏi causation.
