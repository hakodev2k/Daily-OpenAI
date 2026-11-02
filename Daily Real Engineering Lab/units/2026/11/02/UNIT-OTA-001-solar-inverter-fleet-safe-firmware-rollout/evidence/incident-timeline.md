# Synthetic pilot evidence

Các event dưới đây là **sample**, không phải toàn bộ 240 bản ghi; dùng pilot-snapshot.csv để tính tổng.

| Time | Evidence | Context |
|---|---|---|
| 08:00 | campaign=FW-482, pilot targeted=240, 80/region | Campaign starts |
| 08:42 | n-044 attempt=2 ACK SUCCEEDED | Terminal recorded |
| 08:44 | c-014 attempt=1 INSTALLING, gateway timeout | Unknown outcome |
| 08:47 | c-014 attempt=2 FAILED, reason=HEALTH_CHECK | Newer attempt |
| 08:49 | s-031 attempt=1 FAILED, dualBank=false | Recovery unknown |
| 08:55 | CSV snapshot finalized | Point-in-time evidence |
| 09:01 | c-014 attempt=1 ACK SUCCEEDED | Delayed older attempt |
| 09:02 | n-044 attempt=2 ACK SUCCEEDED | Duplicate |
| 09:03 | s-071 offline 3h, OFFLINE_PENDING | No observed install result |
| 09:04 | p95 polling latency 120 ms → 380 ms | Correlation not causation |
| 09:05 | Operator requests 5% wave | Go/no-go decision |

Investigate terminal denominator, stale ACK, duplicate counts, offline reconnect after pause, and evidence distinguishing firmware vs network faults.
