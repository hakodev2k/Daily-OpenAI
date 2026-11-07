# Evidence seed — Partner incident

Dữ liệu synthetic, không có production secret.

| Observation | Direct call | Gateway call |
|---|---|---|
| Initial request | GET /partner/direct | GET /partner/entry |
| Expected business result | 200 | 200 |
| Reported after rollout | 200 | 401 |
| Gateway redirect | n/a | 307 to /partner/secure |
| Service requests | 1 | 1 |
| Service accepted credential | 1 | 0 |

Chưa suy diễn root cause chỉ từ bảng này. Hãy reproduce, quan sát actual counters, sau đó lập ít nhất hai giả thuyết (credential sai, redirect behavior, network/proxy).
