# Incident timeline — synthetic
| Time | Actor | Operation | ETag seen | Result |
|---|---|---|---|---|
| 09:02:01 | A | GET draft | "rev-1" | 200 |
| 09:02:04 | B | GET draft | "rev-1" | 200 |
| 09:03:10 | A | PUT text A | "rev-1" | 200 |
| 09:04:08 | B | PUT text B | "rev-1" | 200 |
| 09:04:15 | A | GET draft | "rev-3" | 200, text A missing |

Dashboard phụ: HTTP 5xx = 0; p95 = 23 ms. Những số này không chứng minh tính toàn vẹn của dữ liệu.
