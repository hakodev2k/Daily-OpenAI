# Reference Solution — chỉ đọc sau khi tự làm

Đây là **một phương án có thể bảo vệ**, không phải kiến trúc duy nhất. Quy định thật phải được pháp chế xác nhận và đối chiếu khả năng cụ thể của storage provider.

## 1. Symptoms
C-101 đã mất file dù case còn mở; C-102 mất file dù hold còn hiệu lực. C-103 có yêu cầu xóa đã đủ điều kiện nhưng vẫn còn file quá SLO. C-104 còn retention nên chưa được xóa; C-105 xóa là hợp lệ theo snapshot nhưng vẫn cần audit chứng minh.

## 2. Evidence
Expiration hiện tại tính từ uploadedAt; chính sách lại tính từ closedAt và hold. CSV chỉ là snapshot: chưa chứng minh nguyên nhân mất object nếu không có storage access logs và lịch sử policy/hold. Đối chiếu audit + object version/tombstone + worker logs để xác minh.

## 3. Root cause / decision
Một global expiration rule không biểu diễn được trạng thái per-case, hold hoặc request pending. Chọn **metadata-driven state machine + storage-enforced protection cho hold/retention quan trọng + deletion worker có reconcile**. Nếu provider không hỗ trợ hold/retention bảo vệ ở tầng storage, phải đánh dấu rủi ro residual, không tuyên bố zero-loss dưới race; có thể tạm dừng automated deletion cho đến khi có enforcement phù hợp.

## 4. State machine và invariant
Trạng thái: ACTIVE → CLOSED_RETAINED → ELIGIBLE → DELETE_PENDING → DELETED_VERIFIED; HOLD có thể là trạng thái chặn độc lập, không ghi đè lifecycle; DELETE_FAILED quay về retry. Mỗi object liên kết case và policyVersion. Eligibility: case=CLOSED AND now >= closedAt+365d AND no active hold AND no other retention constraint. Erasure request tạo request record, không đồng nghĩa xóa ngay.

Invariant: object trong case OPEN hoặc HOLD không được bị delete; hold áp dụng cả object mới. Audit append-only ghi decision, reason, actor, policy version, correlationId. Trạng thái DELETED_VERIFIED chỉ được ghi khi provider xác nhận không còn bản có thể phục hồi theo deletion contract đã định nghĩa; chú ý versioning, soft delete, replication và backups.

## 5. Concurrency & failure handling
Không đủ để worker đọc `holdActive=false` rồi xóa sau đó. Thiết kế cần một **điểm serialize/enforce** giữa hold và delete, tốt nhất là storage-level hold/immutability có hiệu lực trước khi xác nhận hold, kết hợp per-case transaction/lock, generation fencing và recheck trước delete. Với object mới, upload phải nhận protection policy trước khi publish/acknowledge; nếu không thể đảm bảo, không acknowledge upload hoặc block delete trong giai đoạn chưa đồng bộ.

Worker dùng stable operationId, outbox/intent, retry có backoff, verify object/version, reconcile pending intents. Crash sau external delete nhưng trước DB commit → đọc lại trạng thái storage rồi ghi verified, không giả định lệnh đã thành công. Storage API lỗi bất định → reconcile, không đánh dấu DELETED.

## 6. How to verify
Case 1 OPEN → RETAIN; 2 HOLD → RETAIN+PENDING_REQUEST; 3 chưa hết 365 ngày từ close → WAIT; 4 crash → idempotent reconcile; 5 concurrent hold → storage reject deletion hoặc hold serialized trước delete; 6 new upload → protection inherited before visibility. Test thêm failed hold propagation và stale metadata.

## 7. Alternatives / trade-offs
- Global lifecycle: chi phí thấp, đơn giản, nhưng không đáp ứng per-case retention/hold.
- DB-only worker: linh hoạt, dễ query; không thể tự bảo đảm an toàn nếu storage delete và hold activation race, cần external fencing và reconcile.
- Storage-enforced retention/hold + metadata worker: bảo vệ mạnh hơn, nhưng provider semantics, cost, lock configuration, versioning, minimum retention và operational recovery phức tạp. Phải xác minh semantics của provider, đặc biệt chính sách immutable không dễ rút ngắn.

## 8. Wrong fixes
- Tăng TTL toàn cục lên 730 ngày: che triệu chứng, không giải quyết open case hay hold; giữ dữ liệu quá lâu.
- Retry delete không giới hạn: có thể gây duplicate audit hoặc tiếp tục xóa khi hold vừa được bật.
- Đánh dấu deleted ngay khi gửi request: sai trong timeout, 5xx và versioned storage.
- Chỉ thêm `if (!hold)` trước delete: vẫn có TOCTOU race.
Các cách trên có thể dùng như biện pháp phụ trong ngữ cảnh khác nhưng không thỏa contract ở đây.

## 9. Production implications
Backfill closedAt và hold lineage; snapshot inventory để phát hiện object thiếu; chạy shadow policy và dry-run; canary theo case cohort; rollback bằng disable delete worker (không thể rollback file đã xóa). Theo dõi pending age p95/p99, hold-protection latency, delete failures, storage/DB divergence, audit completeness, cost growth. Giữ audit tối thiểu PII, bảo vệ truy cập và phân tách retention audit khỏi retention file.

## 10. Senior engineer should notice
Sự khác nhau giữa **decision correctness**, **enforcement correctness** và **evidence of correctness**. Không tuyên bố bảo đảm pháp lý hoặc xóa hoàn toàn chỉ vì object không còn xuất hiện trong list. Định nghĩa rõ contract với versioning, soft-delete và backup; báo cáo rủi ro khi nhà cung cấp không hỗ trợ.
