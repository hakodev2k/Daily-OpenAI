# Reference solution — Spoiler

Root cause: independent Npgsql pools can request 18 × 30 = **540** connections, but DB app budget is 240 − 40 = **200**. A 170-connection snapshot does not establish a safe bound.

**Option A:** 12 replicas × 15 pool connections = 180, preserving 20 slots of headroom. Bound admission to 60 per pod; use backpressure, explicit timeout budgets and metrics. Copy values from `solution/plan.json` to `workspace/plan.json` then run `pwsh ./verify.ps1`.

**Option B:** PgBouncer transaction pooling may reduce backend connections for short transactions, but check session state, prepared statements, transaction boundaries and pooling mode. It is not an unconditional fix.

**Option C:** Scale DB only after measuring CPU, I/O, memory, locks and cost.

Wrong fixes: increase HPA replicas, increase retry frequency, or only raise `max_connections`. Each can worsen shared resource pressure.

Rollout: apply pool/HPA limits together; canary with synthetic load; monitor connection waits, rejected connections, `pg_stat_activity`, lock waits, queue depth, p95/p99, and booking success. Roll back on connection rejection or latency regression. Add a deployment guardrail for maxReplicas × maxPoolPerPod.

Passing verification proves capacity arithmetic only, not throughput, 800ms SLO, transaction correctness or production safety. Benchmark with a representative workload and real DB before rollout.
