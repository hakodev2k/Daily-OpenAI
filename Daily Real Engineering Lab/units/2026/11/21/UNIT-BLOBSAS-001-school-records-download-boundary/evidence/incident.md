# Evidence packet — dữ liệu giả lập để điều tra

| Window | Downloads/s | p95 API response | App egress | Authorization errors | Storage 5xx |
|---|---:|---:|---:|---:|---:|
| 10:00–10:15 | 8 | 350 ms | 46 MiB/s | 0.2% | 0.01% |
| 12:00–12:15 | 90 | 3.8 s | 490 MiB/s | 0.3% | 0.02% |

- Application CPU: 43% → 58%; memory stable.
- Download endpoint returns HTTP 200 after permission check, then streams full response.
- Storage metrics show median blob read latency 75 ms at both windows.
- Support reports a parent retained a previously shared download link after access removal. Exact link semantics unknown.
- Two unrelated 401 spikes coincided with identity-provider maintenance.

**Investigate:** Does correlation imply causation? Which metric distinguishes App bandwidth from storage latency? Which tests establish the actual revocation guarantee? Estimate 90 × 6 MiB/s and account for traffic mix before interpreting the estimate as measured egress.
