# Evidence packet — Partner throttling

Dữ liệu synthetic; không có token hoặc thông tin cá nhân.

| UTC time | Request | Status | Retry-After raw | Observed planned wait |
|---|---|---|---|---|
| 00:00:00 | G-104 | 429 | \`7\` | 7s |
| 00:00:00 | G-105 | 429 | \`Tue, 27 Oct 2026 00:00:11 GMT\` | 2s |
| 00:00:00 | G-106 | 429 | \`Tue, 27 Oct 2026 00:01:30 GMT\` | 2s |
| 00:00:00 | G-107 | 429 | *(absent)* | 2s |

Client fallback=2s, maxDelay=30s; UTC clock fixed for reproducibility. CPU ~12%, không có dấu hiệu saturation. Một operator nghi ngờ clock skew; cần kiểm chứng hypothesis này thay vì mặc định.
