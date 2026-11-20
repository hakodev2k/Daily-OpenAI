# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
DB có trạng thái Released nhưng thiếu event; event trùng có thể tạo hai shipment. Đây là hai lớp nhất quán khác nhau.

## 2. Evidence
Timeline A cho thấy commit DB đã xong trước khi process chết; timeline B cho thấy ACK ambiguity và consumer redelivery. CPU và HTTP latency không chứng minh được delivery semantics.

## 3. Root cause
Dual write giữa SQL COMMIT và broker publish không có atomic boundary chung. At-least-once delivery và crash trước ACK khiến consumer side effect có thể lặp.

## 4. Phương án đề xuất
Trong **một SQL transaction**, cập nhật `BloodUnits` từ Approved sang Released với optimistic version check, đồng thời INSERT `Outbox(EventId unique, AggregateId, AggregateVersion, Payload, CreatedUtc, Status)`. Chỉ trả thành công sau COMMIT. Background dispatcher đọc outbox chưa gửi theo batch, claim với lease có timeout, publish với stable EventId, đánh dấu sent sau broker ACK. Sau timeout/lease expiry được phép gửi lại. Lưu payload hoặc đủ thông tin tái tạo chính xác **event tại thời điểm commit**, không chỉ đọc trạng thái hiện tại.

Consumer trong **một local transaction** kiểm tra/insert `Inbox(EventId unique)` và tạo shipment với unique business key `(BloodUnitId, Version)`. Nếu key đã tồn tại thì ACK như duplicate; rollback cả inbox và shipment nếu lỗi. Version out-of-order: xác định semantics; với transition cần ordering theo aggregate hoặc buffer/reconcile missing versions; với state projection chỉ áp dụng version lớn hơn bằng atomic conditional update.

## 5. Why it works
Atomic DB commit bảo đảm không tồn tại Released mà thiếu durable publish intent. Dispatcher có thể retry không mất event; consumer idempotency giữ business invariant dù publish/delivery lặp. Đây là **at-least-once + idempotent effects**, không phải exactly-once end-to-end.

## 6. Verification
Điền F1–F10. F1 rollback cả business/outbox; F2 outbox còn pending; F3/F4 resend EventId cũ; F5 inbox+shipment unique bảo vệ; F6 monotonic version; F7 backlog replay; F8 quarantine; F9 lease expiry và unique claim; F10 schema compatibility. Dùng fault injection trên staging trước production, không coi tabletop là runtime proof.

## 7. Alternatives
- Transactional outbox polling: dễ vận hành với SQL Server sẵn có, thêm tải polling và cleanup.
- Change Data Capture: giảm polling, nhưng thêm connector, retention và failure recovery complexity.
- Chỉ gọi broker rồi ghi DB: có thể phát event cho transaction chưa commit; không phù hợp invariant.
- Broker-first + saga: có thể phù hợp workflow phân tán dài, nhưng phức tạp và không thay thế atomic intent ở local transaction.

## 8. Wrong / tempting fixes
- Retry publish trực tiếp sau API failure: không khôi phục được crash ngay sau COMMIT nếu không có durable intent.
- Đánh dấu sent trước publish: có thể mất event.
- Đánh dấu sent sau publish và tin rằng sẽ không trùng: ACK loss vẫn tạo duplicate.
- Chỉ dedupe theo HTTP request ID: consumer redelivery không nhất thiết giữ request context.
- Tăng số replicas: tăng throughput, không sửa consistency boundary.

## 9. Production implications
Theo dõi `outbox_oldest_pending_age_seconds`, `outbox_pending_count`, publish retry/error rate, duplicate consumer rate, DLQ age, end-to-end event latency p50/p95/p99. Alert khi oldest pending >60s hoặc tốc độ backlog tăng; runbook xác minh DB commit, outbox, broker, inbox và replay theo EventId. Retain sent rows đủ lâu cho audit/replay; purge theo policy; bảo vệ PII và secret trong payload; phân quyền dispatcher.

## 10. Trade-offs
Outbox tăng write amplification, table growth và operational monitoring; ordering per aggregate cần thêm coordination. Nếu consumer không cùng DB với shipment side effect, cần idempotency ở đích hoặc reconciliation. Không bảo đảm delivery trong 60s khi broker down 24h; SLO chỉ áp dụng lúc hạ tầng khỏe.

## 11. Senior engineering judgment
Phân biệt atomicity, delivery, processing và side effects; chỉ định rõ invariant; kiểm tra crash windows; thiết kế recovery/observability trước khi chọn product; không đánh đồng broker dedup với business idempotency.
