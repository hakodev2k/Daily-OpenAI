# Investigation guide

1. Run `pwsh ./run.ps1` and record the baseline/burst/candidate table.
2. Calculate DB application budget, worst-case pooled demand and required headroom.
3. Distinguish pool exhaustion from slow queries, lock waits, DB CPU saturation and retry storms using proposed telemetry.
4. Edit `workspace/plan.json`; run `pwsh ./verify.ps1` against learner-editable workspace.
5. Write ADR in `workspace/adr.md`: at least two options, trade-offs, rollout/rollback, SLO risks.
6. Compare with reference after making your own decision.

This is deterministic static capacity replay, not an actual PostgreSQL performance test.
