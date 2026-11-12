# Incident evidence (synthetic)

| Signal | 08:55 | 09:02 | 09:08 |
|---|---|---|---|
| Pods | 4 | 12 | 18 |
| HTTP 5xx | 0.3% | 3.8% | 12.4% |
| p95 | 240ms | 1.4s | 3.9s |

- PostgreSQL `max_connections=240`; reserve 40 connections for administration, workers and other services.
- Npgsql per-pod `Maximum Pool Size=30` for one pool identity. HPA max replicas = 18.
- At 09:08, `pg_stat_activity` sample shows 170 application connections (snapshot, not bound).
- Burst of 900 concurrent HTTP requests; not every request needs a DB connection.
- Target p95 under admitted load < 800ms; maintain at least 20 spare application connection slots.
- CPU per pod decreased during scale-out. Evidence alone does not rule out locks, CPU on DB, or retry storms.

Investigate competing hypotheses and define telemetry that discriminates between them.
