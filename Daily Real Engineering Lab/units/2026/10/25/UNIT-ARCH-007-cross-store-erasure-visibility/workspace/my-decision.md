# My Architecture Decision — UNIT-ARCH-007

## 1. Symptoms and evidence

- User-visible symptoms:
- Timeline events supporting the hypothesis:
- Noise or unknowns:
- Missing evidence to request:

## 2. Invariants and trust boundaries

- What exactly must be true at ACK time?
- What is the authority for public visibility?
- Tenant identity and record identity:
- Legal hold versus public suppression:
- Which reads can bypass our API?

## 3. Options comparison

| Criterion | A: synchronous multi-store delete | B: async + visibility control | C: consolidate read model |
|---|---|---|---|
| Public visibility after ACK | | | |
| Failure recovery | | | |
| Search p95 | | | |
| Operational burden | | | |
| Cost | | | |
| Migration risk | | | |

## 4. Proposed sequence / state machine

Write sequence steps for: DELETE, ACK, search, detail, download, reindex, retry, backup restore. Include state transitions and idempotency key.

## 5. Failure matrix

| Failure | Detection | Safe behavior | Recovery | Owner |
|---|---|---|---|---|
| Reindex from stale snapshot | | | | |
| Worker crash | | | | |
| Redis stale hit | | | | |
| Previously issued attachment URL | | | | |
| Legal hold | | | | |
| Restore from old backup | | | | |
| Region outage | | | | |

## 6. Verification plan

For AC-01 through AC-12, state input, public observable output, metrics and pass/fail. Define at least one synthetic canary and one load test.

## 7. ADR

- Decision:
- Assumptions:
- Why not alternatives:
- Security and privacy consequences:
- Cost and on-call consequences:
- Rollback conditions:
- Unknowns requiring experiments:
- What would change the decision:
