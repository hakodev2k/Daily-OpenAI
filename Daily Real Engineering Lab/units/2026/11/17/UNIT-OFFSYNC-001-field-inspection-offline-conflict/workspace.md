# Investigation Workspace

## Replay hiện tại
| Event | Server version trước | Server version sau | Lost data / invariant |
|---|---|---|---|
| op-a | 10 | | |
| op-b | | | |
| op-b retry | | | |
| op-c | | | |

## Hypotheses và bằng chứng

## Decision matrix
| Policy | Correctness | Cost | Operational risk | Conflict UX |
|---|---|---|---|---|
| LWW | | | | |
| Optimistic concurrency | | | | |
| Field merge + escalation | | | | |

## Contract/API và idempotency

## Tests và observability

## Quyết định, assumptions, trade-offs
