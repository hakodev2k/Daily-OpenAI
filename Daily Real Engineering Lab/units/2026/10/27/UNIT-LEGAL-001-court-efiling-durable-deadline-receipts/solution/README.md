# Reference Solution — SPOILER

> Chỉ đọc sau khi đã phân tích evidence, viết ADR và tự kiểm tra invariants. Đây là **một phương án có thể bảo vệ**, không phải lựa chọn duy nhất.

## 1. Symptoms

Case A: API trả receipt nhưng không còn durable record sau failover. B: hồ sơ được đánh dấu on-time dù commit sau hạn. C: retry sinh receipt thứ hai. D: scanner chậm không làm mất receipt. E: partition nhưng API vẫn trả success khi durability unknown. HTTP success rate cao che khuất lỗi pháp lý.

## 2. Evidence

- A: `api_200_sent` 09:59:58.520Z xảy ra trước failover; không có observed durable commit.
- B: deadline 17:00:00 local = 10:00:00Z; commit 10:00:00.050Z là **50 ms trễ**.
- C: cùng party/key, hai receipt IDs khác nhau.
- D: scanner 45 phút là downstream backlog, không phải chứng cứ receipt bị mất.
- E: `durable-ack=unknown` nhưng HTTP 200, vi phạm tính chắc chắn của lời hứa.
- Metrics CPU chỉ 52%: scale CPU không giải quyết correctness.

## 3. Root cause / systemic failure

Không có một **authoritative, durable acceptance boundary** ràng buộc thời điểm commit, identity và việc phát hành receipt. HTTP response, timestamp verdict, retry và replication acknowledgement bị coi là các nguồn sự thật độc lập.

## 4. One defensible design

```text
Client
  -> Gateway (TLS, auth, request ID, bounded timeout)
  -> Submission API
      -> validate minimal envelope + canonical digest
      -> authoritative transactional store:
         unique (party_id, idempotency_key)
         immutable submission_id + digest
         trusted commit timestamp + deadline verdict
         receipt reference + outbox event in SAME transaction
      -> wait for configured durable quorum acknowledgement
      -> return stable receipt (RECEIVED) only after confirmed commit
  -> asynchronous worker (outbox, at-least-once)
      -> malware/signature/case-management
      -> state transitions VALIDATING / ACCEPTED_FOR_FILING / REJECTED
```

**Data model (conceptual):**

- `Submission(id, party_id, idempotency_key, digest, accepted_at_utc, deadline_verdict, status, receipt_id, version)`
- Unique `(party_id, idempotency_key)`, with immutable digest; canonicalization version recorded.
- `Receipt(receipt_id, submission_id, canonical_digest, accepted_at_utc, verdict, schema_version, signature_or_verifiable_reference)`.
- `SubmissionEvent(sequence, submission_id, event_type, occurred_at_utc, actor, correlation_id)` append-only.
- `Outbox(event_id, submission_id, type, payload_ref, dispatched_at)` in same transaction.
- PII separated from public receipt ID; encrypt storage, authorize lookup, protect signing keys, retention/erasure legal review.

**Clock and deadline:** Define the authoritative acceptedAt as a **trusted server-side commit timestamp with documented semantics**. Do not assume an application clock sampled before commit is equivalent to commit time. If DB-generated timestamp is transaction-start time, it is **not** automatically the commit timestamp. Choose storage/protocol capable of the agreed deadline ordering, or use a trusted sequenced acceptance log whose committed record contains the timestamp and verdict. Legal signs off exact boundary behavior. For B, commit 10:00:00.050Z => late.

**Receipt response:** 200/201 only with confirmed durable commit and stable receipt; 409 for same key/different canonical digest; 503 (or explicit unknown outcome) when commit/quorum cannot be confirmed. A timeout/503 **does not prove rejection**. Client retries same key; server reconciles by authoritative lookup. If a write committed but response lost, retry returns the same receipt. If no quorum, fail closed rather than violate RPO=0.

**Replication:** Use a durable quorum/consensus-backed commit or equivalent synchronous policy across independent failure domains **with verified failover guarantees**. Two independent sites alone do not automatically provide quorum availability under partition. Document fencing, leader election, split-brain prevention, replication lag, backup restore tests, and exact meaning of acknowledged write. If infrastructure cannot meet RPO=0 and latency/availability together, negotiate SLO rather than silently degrade durability.

**Outbox and state:** Workers are at-least-once; consumers idempotent by event ID, retries bounded with dead-letter/manual intervention. Receipt and legal on-time verdict are immutable. Validation outcome changes state and appends an event; it does not erase receipt history. Search indexes and case-management are projections, not authoritative receipt store.

## 5. Why it works

Receipt identity, payload digest, verdict and outbox intent are committed as one logical unit. Durable acknowledgement precedes public promise. Same-key retry converges to one committed identity. Slow downstream processors cannot rewrite or erase evidence of receipt. Fenced failover avoids two independent leaders confirming contradictory records.

## 6. How to verify

- Kill leader between validation and commit: no confirmed receipt.
- Kill leader immediately after confirmed quorum commit but before response: retry returns same receipt and acceptedAt.
- Inject duplicate concurrent same-key requests: unique identity, same digest -> same receipt; changed digest -> 409.
- Freeze clock/drive deadline boundary: 16:59:59.999 commit on-time; 17:00:00.001 late (precision defined by implementation).
- Partition replica and deny quorum: fail closed; no 200. Heal partition, reconcile unknown requests.
- Disable scanner 45 min: `RECEIVED` remains queryable, queue age alarm fires, processing resumes idempotently.
- Restore backup/fail over site: all acknowledged receipt IDs still queryable; compare durable log sequence and hashes.

## 7. Alternative solutions

**Option A — Single authoritative SQL store with synchronous HA and transactional outbox:** familiar operations, strong transactional boundary, relatively low team overhead; latency depends on quorum and storage topology. Confirm commit timestamp and replica acknowledgement semantics for chosen engine.

**Option B — Consensus-backed append-only receipt log plus asynchronous projections:** clear ordering/audit and replay, scalable event processing; more operational complexity, careful design needed for idempotency uniqueness and read-your-write receipts. Never assume generic async event bus guarantees RPO=0.

Either can satisfy requirements **only with verified infrastructure and explicit failure contract**. Do not choose based on architecture fashion.

## 8. Wrong / tempting fixes

- **HTTP 200 after enqueue to an in-memory queue:** improves latency but can lose receipts on crash.
- **Async replication + best-effort failover:** may lose acknowledged writes; contradicts RPO=0.
- **Gateway retries with new request IDs as dedup key:** creates duplicate submissions.
- **Use client timestamp to honor deadline:** susceptible to skew/manipulation.
- **Block submission until malware scan completes:** couples legal receipt to an external outage.
- **Raise instance count:** treats correctness failure as capacity failure.
- **Always return 409 on timeout retry:** cannot distinguish already committed write from unknown state.

## 9. Production implications & trade-offs

Budget for peak 300 rps (32 KiB envelope excludes attachments), transaction throughput, commit p95/p99, quorum loss and query reconciliation. SLOs must include `confirmed_receipt_without_durable_record = 0`, duplicate identity count = 0, deadline verdict disagreement = 0, commit latency, unknown-outcome rate, outbox age and replay lag. Alert on correctness invariants even when HTTP availability appears healthy. Shadow-evaluate receipts before cutover; dual-write without authoritative ordering is dangerous. Test failure injection in staging and practice rollback/reconciliation runbook.

Availability and RPO are competing objectives during partition: **preserve legal correctness and explicitly show degraded availability**.

## 10. What a Senior / Architect should notice

The hard question is not "which database?" but **what legal promise does a receipt make, at what instant, and what failure evidence proves that promise after the primary disappears?** Separate authoritative state from projections; force Product/Legal to approve deadline and unknown-outcome semantics; document infrastructure limits, not just happy-path diagrams.

## Extension reference

For the new ingress-time rule, issue a fast `PENDING_TRACKING` token **without claiming legal receipt**. A trusted ingress event must itself be durably committed (or protected by an equivalent verifiable ingress log) to prove arrival before deadline. If the 250 ms token is only an ephemeral acknowledgement, it cannot by itself establish legal on-time status after site loss. Confirmed receipt follows durable proof and replay-safe reconciliation. If partition prevents durable proof within 250 ms, present pending/unknown and renegotiate guarantees; never mislabel tracking as a legal receipt.
