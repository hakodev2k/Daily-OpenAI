# Reference Solution — SPOILER

## 1. Symptoms
Two API instances each approve capacity based on an earlier snapshot; an ambiguous timeout causes a second loan and DRM entitlement.

## 2. Evidence
Timeline A shows both readers saw 1/2 active before either committed. Timeline B shows op-C retried after a side effect was accepted but before response delivery. CPU, CDN and DB latency are distractions.

## 3. Root cause
The check and allocation are not one serialized admission operation. Separately, operation identity is not enforced across retries. A database transaction alone at READ COMMITTED without a serialization mechanism does not guarantee the cross-row capacity invariant.

## 4. Recommended design
Use PostgreSQL as source of truth. Schema sketch:
- `license_pool(isbn PK, capacity, reserved_count, version)`; check `0 <= reserved_count <= capacity`.
- `loan(id PK, isbn, patron_id, operation_id UNIQUE, request_hash, state, expires_at)`.
- Partial unique index on `(isbn, patron_id)` for states that occupy a license (e.g. RESERVED, ACTIVE).
- `outbox(event_id PK, loan_id, type, payload, status, attempts)` with stable external idempotency key.
- `drm_receipt(loan_id UNIQUE, external_entitlement_id UNIQUE, status)`.

One transaction on primary: check existing operationId and matching payload hash; lock pool row `SELECT ... FOR UPDATE` (or atomic conditional `UPDATE ... WHERE reserved_count < capacity RETURNING`); reserve slot, insert loan in RESERVED, append outbox; commit. Under duplicate operationId unique conflict, re-read committed original and return its state. Avoid an external DRM call inside the SQL transaction.

Outbox worker grants DRM using `loanId` as stable external idempotency key. If DRM supports idempotency, retries reuse it; if not, query entitlement status before replay and quarantine ambiguous outcomes for operator reconciliation. Mark ACTIVE only after confirmed entitlement. RESERVED consumes capacity, so concurrent grants cannot oversubscribe. API may return PENDING_RECONCILIATION until confirmed. Final GRANTED only after confirmed DRM receipt. Returning/canceling must also coordinate DRM revoke, expiration and capacity release; do not release a license while an unrevoked entitlement still authorizes access.

## 5. Why it works
Pool-row serialization protects aggregate capacity across instances. Unique operationId protects retry identity; payload hash prevents accidental reuse for another command. Transactional outbox closes the commit-to-publish gap without pretending to have a distributed SQL+DRM transaction.

## 6. Verification
V1: op-A reserves remaining slot; op-B sees full pool and waits. V2: retry reads same loan. V3: mismatch returns 409. V4–V5: worker replays stable key or quarantines uncertainty. V6: release only after revocation confirmed; waiting borrower admitted through same serialized path. V7: contention increases latency but not capacity. V8–V9: all admission decisions use primary. V10: disable new grants and reconcile historical anomalies.

## 7. Alternatives
- Serializable transactions with bounded retry on serialization failure: defensible, but contention/retry behavior needs measurement.
- Atomic conditional counter update: efficient, provided every release/expiration path preserves counter/loan consistency.
- Distributed lock (Redis): not sufficient as sole correctness boundary; lock lease expiry and failover can overlap.
- Single-ISBN command queue: can simplify ordering, but durable delivery, partition ownership and reconciliation increase operations cost.

## 8. Wrong fixes
- Add `lock` in ASP.NET process: does not coordinate multiple instances.
- Cache active count: introduces stale admission data.
- Increase pool capacity: breaches contract.
- Retry DRM with a fresh request ID: duplicates entitlements.
- Return GRANTED immediately after DB commit: conflates reserved capacity with external right to read.

## 9. Production implications and trade-offs
Row contention on a hot ISBN is intentional correctness serialization. Track lock-wait p95, reservation-to-active age, outbox retries, orphan DRM receipts, invariant violations and operation conflicts. Set bounded retries, dead-letter/quarantine, audit trail and manual repair runbook. Roll out behind feature flag after reconciling pre-existing loans and backfilling operation IDs; use shadow evaluation before enabling writes. Rollback application deployment must not re-enable unsafe writes or discard durable pending events.

## 10. Senior / architect judgment
Define the business invariant before selecting infrastructure. Distinguish logical loan, reserved capacity and external entitlement. Require documented semantics for DRM idempotency/revoke before promising exactly-once effects. Explain consistency vs latency, operational staffing, failure recovery and auditability rather than recommending more infrastructure by default.
