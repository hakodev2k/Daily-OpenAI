# Reference Solution — SPOILER

## Symptoms and evidence
LOT-A17 bị recall 09:04 nhưng ALLOW 09:16 với projection cũ r11. LOT-C19 cũng ALLOW khi Registry timeout. Event r11 đến lại sau r12.

## Root cause
Read model eventually consistent bị sử dụng như authority tại điểm dispatch không thể đảo ngược; fail-open khi thiếu bằng chứng. Đây không chỉ là vấn đề độ trễ broker.

## One defensible fix
Tách picklist (có thể stale) và dispatch authorization. Registry hoặc một admission authority strongly consistent phải serialize recall với dispatch commit. Trạng thái RECALLED terminal; incoming revision chỉ áp dụng nếu lớn hơn revision đã lưu. DispatchId là idempotency key, ghi audit lot/revision/decision. Nếu không lấy được bằng chứng có thẩm quyền: HOLD và kích hoạt manual substitution.

Một lần đọc Registry rồi cho xe đi chưa đủ: recall có thể commit giữa check và physical release (TOCTOU). Thiết kế cần transaction/fencing protocol hoặc physical gate kiểm tra authorization còn hiệu lực ngay tại điểm release. Recall và dispatch phải có thứ tự serialize rõ ràng; nếu recall thắng, dispatch bị từ chối. Nếu dispatch đã hoàn tất trước recall, cần traceability và recall response hậu dispatch.

## Verification
Chạy acceptance A–G trên timeline; kiểm tra hai interleavings concurrent recall/dispatch, không cho commit dispatch sau recall đã commit. Duplicate và retry không phát sinh xuất kho hai lần. Bulk 400 lots phải bị chặn ngay cả khi projector chậm.

## Alternatives
1. Synchronous authoritative admission: dễ chứng minh invariant; tốn latency và phụ thuộc availability.
2. Strongly consistent safety-local authority với quyền ghi/recall được đồng bộ theo protocol: giảm latency nhưng phức tạp vận hành.
3. Async cache invalidation/short TTL: rẻ nhưng không bảo đảm invariant nghiêm ngặt trong outage.

## Wrong fixes
Tăng tốc broker, giảm cache TTL, thêm retry hoặc giả định exactly-once không xóa được safety gap. Signed approval token cấp trước recall mà không có revocation cũng không giải quyết race.

## Production implications and trade-offs
Metrics: recall-to-block latency, unsafe-admission attempts, HOLD rate, authority timeout, event lag, reconciliation discrepancies. Drill broker outage, bulk recall, Registry 90s outage, retry, concurrent commit. Fail-closed hy sinh availability có chủ đích; cần quy trình hold thủ công.

## Senior engineer should notice
Phải định nghĩa chính xác dispatch commit và physical release, trách nhiệm vận hành, auditability và chính sách xử lý lô đã rời kho trước recall. Đây là quyết định safety và business, không chỉ là message consumer fix.
