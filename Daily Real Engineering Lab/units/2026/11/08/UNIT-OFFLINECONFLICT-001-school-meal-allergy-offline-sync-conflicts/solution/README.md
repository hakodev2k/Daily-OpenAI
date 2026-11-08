# Reference solution — spoiler

## Root cause
Server chọn last-write-wins theo `clientUpdatedAt` không đáng tin và cho phép offline edit trở thành safety clearance. E3 timestamp tương lai khiến E6 có thể overwrite restriction revision 41. Duplicate retry làm audit lặp nếu không dedupe.

## Invariants
Restriction mới chỉ được gỡ bằng quyết định có thẩm quyền, dựa trên revision hiện hành, có audit. Offline snapshot không chứng minh trạng thái mới nhất. CommandId chỉ được xử lý một lần. Server revision tăng đơn điệu.

## Recommended architecture
Safety state do server quản lý. Tablet giữ encrypted read-only snapshot `serverRevision`, `lastConfirmedAt`, `syncStatus`. Offline edits tạo `ProposedCorrection` chờ review, không trực tiếp clear restriction. `POST /students/{id}/proposals` gồm `commandId`, `baseRevision`, `actorId`, `deviceId`, `reason`, `proposedChange`. Unique(commandId, actorId) bảo đảm idempotency, audit transaction. Nếu stale revision trả `409 Conflict` với revision hiện hành và redacted details. Chỉ role được phép mới gọi `POST /students/{id}/clearance-decisions` với `expectedRevision` và evidence.

## Replay
E2 authoritative state rev41 restricted. E3 là pending proposal. E4 hold/manual supervised verification. E5 duplicate deduped. E6 rev41 vẫn restricted.

## Acceptance matrix
| Case | Expected |
|---|---|
| T1 | Restriction rev41 thắng snapshot rev40 |
| T2 | Offline clearance chỉ tạo proposal |
| T3 | Same commandId -> one logical proposal |
| T4 | Reordering không overwrite safety state |
| T5 | Client timestamp chỉ để diagnostics |
| T6 | Offline uncertain -> hold/manual verification |
| T7 | 409 stale revision; no silent overwrite |
| T8 | Authorized actor + evidence + current revision -> audited rev42 clearance |

## Alternatives
Last-write-wins dễ nhưng không đạt safety. Generic optimistic concurrency tốt cho non-safety edits nhưng chưa đủ nếu tự động chấp nhận clearance. Server-authoritative safety tốn workflow review nhưng bảo vệ invariant.

## Wrong fixes
NTP không bảo đảm trusted clock. Retry thêm không giải quyết semantic conflict. Server receipt last-write-wins vẫn có thể nhận stale intent muộn. Cache TTL ngắn không chứng minh safety khi offline.

## Operations
Canary một trường; shadow-compare old/new decisions. Metrics: stale_revision_conflicts, offline_scans_held, unauthorized_clearance_attempts, duplicate_commands_deduped, sync_lag_seconds. Rollback client UI nhưng không rollback server-side safety invariant. Encrypt local storage, revoke device tokens, minimize retention.
