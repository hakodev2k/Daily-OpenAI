# Incident packet: volunteer shift report

## Timeline (synthetic)
- 08:00: 30.000 assignments loaded, covering index created.
- 08:10: metadata sync updates 5.000 assignments without changing query filters.
- 08:11: heap reads rise; production p95 is not measured by this local fixture.
- 08:15: proposal to add another index, without an EXPLAIN comparison.

## Investigation tasks
1. Record `Index Only Scan`, `Actual Rows`, `Heap Fetches`, and buffers before/after.
2. Compare DDL and table maintenance configuration, not just the SQL query.
3. Consider four hypotheses: index columns missing; planner switched plans; visibility of heap pages changed; external I/O contention.
4. Gather evidence to reject at least one hypothesis.
5. Define an operational policy for a table with recurring updates.

## Commands (PowerShell)
```powershell
docker exec relab-pgvis-20261029 psql -U lab -d labdb -c "SELECT indexname,indexdef FROM pg_indexes WHERE tablename='volunteer_shift_assignments';"
docker exec relab-pgvis-20261029 psql -U lab -d labdb -c "SELECT reloptions FROM pg_class WHERE relname='volunteer_shift_assignments';"
docker exec relab-pgvis-20261029 psql -U lab -d labdb -c "SELECT n_tup_upd,n_dead_tup,last_vacuum,last_autovacuum FROM pg_stat_user_tables WHERE relname='volunteer_shift_assignments';"
```

## Constraints
- Preserve all 5.000 matching IDs.
- Do not remove the covering index or hide the symptom by changing the query.
- A query plan node named `Index Only Scan` is not sufficient proof of zero heap access.
- Distinguish a one-time repair from a sustainable production policy.
- Local timings are not a benchmark or evidence of production latency.
