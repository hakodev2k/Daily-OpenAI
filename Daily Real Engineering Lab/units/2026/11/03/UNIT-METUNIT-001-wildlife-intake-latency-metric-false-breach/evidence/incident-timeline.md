# Incident timeline — synthetic evidence

| Time | Source | Observation |
|---|---|---|
| 09:02 | Release | Instrumentation updated; business handler unchanged |
| 09:06 | Independent traces | 120 ms, 180 ms, 240 ms, 300 ms, 600 ms |
| 09:07 | Dashboard | intake.processing.duration, unit=s, p95=600.000 |
| 09:07 | Alert | Breach if p95 > 0.750 s |
| 09:10 | Access log | 5/5 successful requests; no retry |
| 09:12 | Host | CPU 22%, no significant GC pause |

Trace and histogram are independent signals. Determine whether workload or telemetry is misleading before proposing rollback.
