# My investigation and ADR

## Replay
- E6 current state:
- Violated invariant:
- Hypotheses and evidence:

## Decision matrix
| Option | Safety | Offline UX | Complexity | Audit | Cost |
|---|---|---|---|---|---|
| Last-write-wins | | | | | |
| Revision conflict + manual review | | | | | |
| Server-authoritative safety | | | | | |

## Protocol
- Read model / freshness:
- Write command / idempotency key:
- Expected revision / 409 response:
- Offline serving:
- Auth, encryption, retention:

## Verification
| Case | Expected server state | Device behavior | Evidence |
|---|---|---|---|
| T1 New restriction while offline | | | |
| T2 Offline attempted clearance | | | |
| T3 Duplicate upload | | | |
| T4 Reordered commands | | | |
| T5 Clock skew | | | |
| T6 Offline meal scan | | | |
| T7 Reconnect stale revision | | | |
| T8 Authorized clearance | | | |

## Rollout / rollback / observability
- Canary:
- Metrics:
- Rollback:
- ADR rationale:
