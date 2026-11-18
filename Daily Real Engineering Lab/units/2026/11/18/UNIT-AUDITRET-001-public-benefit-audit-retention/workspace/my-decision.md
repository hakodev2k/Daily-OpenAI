# ADR của tôi — chưa điền

## Quyết định và trạng thái
Proposed / Accepted / Deferred:

## Facts vs assumptions
- Facts:
- Unknowns:
- Assumptions:

## Các phương án và trade-offs
| Option | Query SLA | Retention / erasure | Consistency | DR | Cost / operations |
|---|---|---|---|---|---|
| A: PostgreSQL partitions | | | | | |
| B: Separate audit storage | | | | | |
| C: Hybrid | | | | | |

## Capacity model
Events/year, 7-year total, KiB→GiB, multiplier cho WAL/backup/replicas, sensitivity ±50%:

## Data flow và transaction boundary

## Authorization, encryption, access logging và tamper evidence

## Migration, dual-read/dual-write, reconciliation và rollback

## 3 experiments: input, expected metric, pass/fail

## Change request: regional outage + 30-second retrieval

## Decision rationale và conditions for reconsideration