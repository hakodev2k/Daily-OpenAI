# My Architecture Investigation — UNIT-LEGAL-001

> Làm trên bản sao cá nhân. Không đọc solution trước khi điền.

## 1. Symptom và evidence

| Case | Observed | Hypothesis | Evidence cần bổ sung | Confidence |
|---|---|---|---|---|
| A | | | | |
| B | | | | |
| C | | | | |
| D | | | | |
| E | | | | |

## 2. Invariants (ít nhất 6)

1.
2.
3.
4.
5.
6.

## 3. State machine và sequence

Mô tả transitions, acceptedAt, commit point, idempotency, retry và failover. Vẽ Mermaid hoặc ASCII ở đây.

## 4. Data ownership & durability

Schema tối thiểu, unique constraints, receipt proof, trusted clock, site/quorum policy, audit/retention.

## 5. Architecture options

| Criterion | Option A | Option B |
|---|---|---|
| Receipt correctness | | |
| RPO / failover | | |
| Latency at peak | | |
| Operational complexity | | |
| Cost / team fit | | |
| Failure behavior | | |

## 6. ADR

- Decision:
- Context:
- Chosen option and why:
- Rejected alternatives:
- Consequences / accepted risk:
- What would change the decision:

## 7. Failure-test matrix

| Fault | Trigger | Expected external response | Durable evidence | Recovery assertion |
|---|---|---|---|---|
| Site dies after API begins | | | | |
| Deadline boundary | | | | |
| Client retry after timeout | | | | |
| Scanner offline | | | | |
| Inter-site partition | | | | |
| Same key different payload | | | | |

## 8. SLO, alerting, rollout

Metrics, burn-rate alerts, failure injection, shadow receipts, rollback, runbook.

## 9. Requirement change

Đọc `extensions/change-request.md` và ghi revised decision.

## 10. Verification result

Checklist đã pass/chưa pass, open questions, giả định cần Legal/Product ký duyệt.
