# Decision record — UNIT-REPLALAG-001

## Symptoms / impact
Fill in request IDs violating the contract and why.

## Evidence
Separate direct evidence from assumptions.

## Hypotheses
1.
2.
3.

## Options matrix
| Option | Correctness | Availability | Latency | Primary load | Operational risk |
|---|---|---|---|---|---|
| All reads on replica | | | | | |
| All reads on primary | | | | | |
| Targeted consistency-aware read | | | | | |

## Selected design
Specify request/revision token, authorization, routing, fallback, status semantics, timeouts, and bounds.

## Failure-path acceptance tests
| Case | Expected response | Evidence / assertion |
|---|---|---|
| Replica revision behind required | | |
| Replica caught up | | |
| Primary unavailable | | |
| No recent write token | | |
| Retry after 503 | | |
| Unauthorized student | | |

## Metrics and rollout
Define alert thresholds, canary, rollback, and load budget.

## Remaining uncertainties
