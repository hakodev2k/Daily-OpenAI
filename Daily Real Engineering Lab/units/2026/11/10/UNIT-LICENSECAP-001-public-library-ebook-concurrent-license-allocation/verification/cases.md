# Tabletop verification contract

Không chạy code tự động: đây là design/architecture tabletop. Điền kết quả cho từng case bằng state machine và transaction boundary đã đề xuất; một case PASS chỉ khi có bằng chứng invariant, không chỉ nói "dùng transaction".

| ID | Input / interleaving | Must hold |
|---|---|---|
| V1 | BK-41 1/2 active; op-A và op-B cùng lúc | exactly one additional GRANTED, other WAITLISTED |
| V2 | op-C commit, response lost, retry same ID | exactly one logical loan, stable response |
| V3 | same operationId but changed ISBN/patron | reject conflict; no new loan |
| V4 | DRM timeout after durable loan decision | no blind duplicate grant; recoverable pending state |
| V5 | worker crashes after DRM succeeds before marking delivered | replay safe; reconcile with DRM by stable external key |
| V6 | loan returned while queued borrower waits | capacity reusable exactly once; no negative counter |
| V7 | 50 req/s one hot ISBN, 2 licenses | correctness under contention; bounded queue/retry budget |
| V8 | DB failover between commit and response | reconcile from durable primary before finalizing |
| V9 | read replica lag 5 seconds | admission decisions never use stale replica |
| V10 | migration from existing loans with 3 active / cap 2 | quarantine anomaly; no further grants until repaired |

## Replay procedure
For each case: (1) write initial state; (2) list ordered transaction/DRM events; (3) show final rows and API statuses; (4) prove invariants; (5) list failure-recovery path. Reject an architecture if any case lacks a credible recovery mechanism.

## Expected outcomes
Starter timeline A yields 3 active loans, B yields 2 loans for op-C. A correct proposed design yields <=2 active loans for BK-41 and one logical loan per operation. Timing is not graded; correctness and recoverability are.
