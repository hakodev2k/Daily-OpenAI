# Dữ liệu response giả lập

| Batch | Status | Retry-After | Received UTC | Remaining budget |
|---|---:|---|---|---:|
| OBS-041 | 429 | `Wed, 04 Nov 2026 10:00:08 GMT` | 10:00:00Z | 30s |
| OBS-042 | 503 | `Wed, 04 Nov 2026 10:00:12 GMT` | 10:00:00Z | 30s |
| OBS-043 | 429 | `7` | 10:00:00Z | 30s |
| OBS-044 | 429 | `Wed, 04 Nov 2026 10:00:45 GMT` | 10:00:00Z | 20s |
| OBS-045 | 503 | `later` | 10:00:00Z | 30s |

Quan sát từ lần chạy starter: các batch OBS-041, OBS-042, OBS-044 có quyết định retry khác hợp đồng đối tác. Thông số trong harness là synthetic; không có request HTTP thực.
