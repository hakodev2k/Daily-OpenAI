# Incident timeline (simulated)

| Logical tick | Event | Observation |
|---:|---|---|
| 0 | worker-A starts route calculation | A acquired permission |
| 10 | A permission reaches its configured duration | A is still calculating |
| 11 | worker-B starts the same route | B acquired new permission |
| 11 | B publishes its result | store shows plan-from-B |
| 11 | A finishes delayed work | store shows plan-from-A in starter |

Các tick là đồng hồ mô phỏng, không phải thời gian wall-clock. Hãy đối chiếu với `scripts/run.ps1` và xác định invariant mong muốn.
