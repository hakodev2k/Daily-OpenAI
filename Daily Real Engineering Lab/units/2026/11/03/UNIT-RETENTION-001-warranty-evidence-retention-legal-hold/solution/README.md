# Reference Solution — Spoiler

Đây là **một thiết kế có thể bảo vệ**, không phải đáp án duy nhất.

## 1. Symptoms
C101 quá hạn còn object; C105/C106 có dấu hiệu orphan; C107 thiếu file dù hold active; C108 có race giữa hold ACK và message cũ. C104 eligible nhưng chưa breach tại 03:13.

## 2. Evidence & decision table
`retentionEnd=closedAt+30 ngày`. Nếu hold ACTIVE: không có eligibleAt. Nếu RELEASED: `eligibleAt=max(retentionEnd,releasedAt)`; nếu NONE: `eligibleAt=retentionEnd`. `deadlineAt=eligibleAt+72 giờ`.

| Claim | Eligible / deadline (+07) | Classification / action |
|---|---|---|
| C101 | 01/10 10:00 / 04/10 10:00 | Overdue: xóa có kiểm soát, báo SLA |
| C102 | 19/11 10:00 / 22/11 10:00 | Chưa eligible: giữ |
| C103 | Không có khi hold ACTIVE | Bảo vệ object |
| C104 | 31/10 08:00 / 03/11 08:00 | Eligible, chưa breach: xử lý trước deadline |
| C105 | 20/10 08:00 / 23/10 08:00 | Overdue, timeout mơ hồ: kiểm tra/retry idempotent |
| C106 | 15/10 10:00 / 18/10 10:00 | Orphan: reconcile từ tombstone bền vững |
| C107 | Không có khi hold ACTIVE | Integrity incident: không trả AVAILABLE |
| C108 | 15/10 10:00 / 18/10 10:00 trước hold | Message cũ phải bị chặn sau ACK hold |

## 3. Root cause hypothesis
Quyết định retention, DB purge, hold update và object deletion độc lập tạo race và mất bằng chứng đối chiếu. Đây là giả thuyết dựa trên replay; cần trace/code để kết luận nguyên nhân cụ thể trong production.

## 4. Proposed fix
PostgreSQL giữ authoritative `claim_retention` và `object_manifest`: `closedAt`, `holdState`, `holdVersion`, `policyVersion`, `eligibleAt`, `deadlineAt`, `deletionState`, `attemptId`, `objectKey/version`, `lastReceipt`. Không tái sử dụng object keys.

State machine: `AVAILABLE → ELIGIBLE → DELETE_PENDING → DELETING → DELETED`, với `HELD` là gate và `EVIDENCE_MISSING` là integrity state. DB transaction chỉ admit DELETING khi no active hold, đủ retention, đúng version và lease. Hold acceptance phải serialize với admission trên cùng claim: nếu delete chưa admit, cập nhật hold/version rồi mới ACK; worker message cũ fail compare-and-swap. Nếu đã DELETING, hold API trả PENDING/CONFLICT và chỉ ACK sau khi chứng minh file còn và hold đã được áp dụng. Không thể ACK retroactive sau physical delete.

Remote delete ngoài DB transaction; timeout là ambiguous outcome. Retry idempotent theo object version, kiểm tra kết quả, commit receipt và tombstone. Worker crash/duplicate dùng lease và state version, không cho stale worker ghi đè. Reconciliation định kỳ đối chiếu manifest/tombstones với storage, phát hiện orphan, missing-held và overdue. Không xóa tombstone quá sớm.

## 5. Why it works
DB là serialization point cho hold/delete admission, còn reconciliation và idempotency giải quyết kết quả không thể commit atomically giữa DB và object storage.

## 6. How to verify
Mental replay A1–A12; đặc biệt C108 hold ACK trước worker wake-up → worker không được delete; C105 timeout sau delete → retry hội tụ; C107 missing-held → incident. Trước production dùng fault injection, concurrent workers, storage timeout và backup restore drill.

## 7. Alternative fixes
- **Storage-native lifecycle:** vận hành nhẹ nhưng không hiểu closedAt/hold trong DB nếu thiếu đồng bộ tags và ordering guarantees.
- **Cron DB → delete:** ít code, nhưng race, mất receipt và crash recovery yếu nếu không thêm durable state.
- **DB state machine + workers + reconciliation:** kiểm chứng tốt hơn, đổi lại cần monitoring và logic phức tạp.
- **Event-sourced retention service:** truy vết mạnh nhưng thường quá nặng với team hiện tại; vẫn không tạo cross-store atomicity.

## 8. Wrong / tempting fixes
Tăng cron frequency chỉ giảm backlog; tin 200/204 không chứng minh object versions đã xóa; xóa DB trước blob làm mất manifest; áp TTL chung có thể bỏ qua hold; thêm retry mà thiếu version fencing vẫn cho message cũ xóa sau ACK.

## 9. Production implications
Metrics: `eligible_backlog`, `overdue_count`, `oldest_eligible_age`, `held_missing_count`, `orphan_count`, `ambiguous_outcomes`, `hold_pending`, `reconciliation_lag`. Alert P1 khi held object missing; cảnh báo SLA trước deadline. Audit ghi policy/hold version và receipt, phân quyền tối thiểu, không ghi PII vào labels.

Immutable backups có thể chứa dữ liệu cũ đến khi hết backup retention. Không cam kết secure erase trên mọi backup nếu không chứng minh được. Khi restore, replay tombstones/holds và reconcile trước khi mở serving.

## 10. Trade-offs & Senior insight
Phân biệt eligibleAt với deadlineAt (C104), ACK hold với request hold, metadata DELETED với physical deletion, và integrity incident với cleanup backlog. State machine không phải chân lý tuyệt đối: thay đổi WORM, multi-region hay compliance phải đánh giá lại giả định.
