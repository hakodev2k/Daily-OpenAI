# Senior challenge — production constraints
Bổ sung yêu cầu 5 replicas, gateway 429 Retry-After, restart giữa send và ACK, 100k message/ngày và ordering chỉ trong từng site.

Thiết kế persisted delivery state, lease/visibility timeout, idempotency key, backoff+jitter, redrive ownership, per-site fairness, SLO và runbook. Chỉ ra tình huống nào cần partitioned broker, khi nào DB queue đủ, và cách kiểm chứng không mất message sau crash. Không tuyên bố exactly-once nếu chưa giải quyết atomicity với side effect bên ngoài.
