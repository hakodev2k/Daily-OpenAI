# My Decision — UNIT-DBM-001

> Không mở solution trước khi tự điền.

## 1. Symptom & evidence
- Symptom:
- Evidence đủ / còn thiếu:
- Hypotheses (ít nhất 3):

## 2. Version × schema compatibility
| App version | S0: legacy only | S1: both columns | S2: new only |
|---|---|---|---|
| V1.0 | | | |
| V1.1 | | | |
| V2 | | | |

Giải thích riêng read, write, và tính đúng dữ liệu sau concurrent update.

## 3. Candidate strategies
| Tiêu chí | Rename/drop sớm | Expand–migrate–contract | Phương án khác |
|---|---|---|---|
| Correctness | | | |
| Rolling coexistence | | | |
| Rollback | | | |
| Operational complexity | | | |
| Lock/backfill cost | | | |

## 4. Chosen plan
- Phase 0 baseline / observability:
- Phase 1 schema:
- Phase 2 compatibility app:
- Phase 3 backfill / cutover:
- Phase 4 contract:
- Authoritative field at each phase:
- Rollback target and procedure:
- No-go gates:

## 5. Verification (ít nhất 6 test cases)
| Test | Setup | Expected result | Evidence |
|---|---|---|---|
| Mixed-version GET | | | |
| Mixed-version PUT | | | |
| Old writer after backfill | | | |
| Backfill interrupted | | | |
| V2 → compatibility rollback | | | |
| Tenant boundary | | | |
| Optimistic concurrency | | | |

## 6. Requirement change: rollback 72h
- Gate nào phải đổi?
- Rủi ro và chi phí phát sinh?

## 7. Decision & trade-offs
- Decision:
- Rejected alternatives:
- Risks accepted:
- Signals that would reverse decision:
- What I learned:
