# Reference Solution — chỉ đọc sau khi tự làm

## Symptoms
Trong rehearsal, search v2 healthy nhưng trả kết quả thiếu hồ sơ mới và revision cũ.

## Evidence
Snapshot v2 chốt seq 500; v1 đã tới seq 503 tại 09:03. Events 501–505 được ghi trong DB sau checkpoint, trong đó seq 504 gỡ công khai một bản ghi. Document count không chứng minh tính mới của từng record; health green chỉ phản ánh trạng thái cluster.

## Root cause
Rebuild in-place hoặc chuyển read sang snapshot v2 trước khi catch up dẫn tới cửa sổ index thiếu các commit phát sinh sau snapshot. Nếu replay thiếu kiểm soát revision, event đến trễ còn có thể ghi đè dữ liệu mới.

## Một thiết kế có thể bảo vệ được
1. Chụp snapshot DB với một checkpoint change-stream nhất quán (seq 500). Tạo index-v2 riêng; v1 tiếp tục phục vụ reads.
2. Ingest snapshot vào v2, sau đó replay tất cả change events seq >500. Mỗi event mang recordId, revision và visibility; apply idempotent, từ chối revision thấp hơn đã thấy. Tombstone cho withdrawn/delete không được bỏ qua.
3. Đọc watermark liên tục; chặn cutover nếu gap, replay lag vượt 30s, missing tombstone, reconciliation mismatch hay p95/error budget bị vượt. Watermark toàn cục chỉ có ý nghĩa khi không còn sequence gap; kiểm tra per-record revisions và sample queries.
4. Dùng alias/read routing đổi atomically sang v2 sau gate; theo dõi 5xx, p95, lag, tỷ lệ mismatch và withdrawn exposure. Giữ v1 để rollback có thời hạn.
5. Trong dual-run, tiếp tục đồng bộ changes cho cả v1 và v2. Nếu rollback về v1, xác nhận v1 đã catch up và mọi withdrawal tombstone hiện hành được áp dụng; nếu không, ưu tiên tắt truy vấn nhạy cảm/fail closed thay vì lộ hồ sơ.
6. Định nghĩa timeout 5 phút cho quyết định rollback, owner trực ca, kịch bản stream outage và cutover abort. Sau 48h, xác minh lag=0, retention và cleanup v1.

## Timeline và adversarial replay
- 09:03: v2 chưa đầy đủ; không đổi read.
- 09:05: count 2,400,000 và health green không đủ; seq 501–505 cần replay và đối chiếu.
- 09:06: nếu v2 vẫn seq 500, abort cutover; tiếp tục v1.
- Event 505 (R-0420 revision 9) trước 502 (revision 8): giữ revision 9. Seq 504 retry hai lần: tombstone idempotent, R-0077 không được hiển thị.

## Why it works
Snapshot + checkpoint xác định phần dữ liệu nền; replay đóng khoảng trống cập nhật. Version/revision guard ngăn event đến trễ đảo ngược trạng thái. Atomic alias tránh đọc index đang build. Reconciliation xác minh dữ liệu chứ không chỉ trạng thái cluster.

## How to verify
Điền từng hàng V1–V10 bằng evidence; thử stream gap, duplicate, reordering, tombstone và rollback lag. Test cutover chỉ PASS khi tất cả gates PASS.

## Alternative fixes
- Tạm ngừng ghi rồi rebuild: đơn giản nhưng vượt giới hạn 10 giây, không phù hợp.
- Dual-write trực tiếp từ application: có thể giảm lag nhưng tạo dual-write failure modes; vẫn cần durable replay/reconciliation.
- Dùng DB search tạm thời: hợp lệ nếu query volume, ranking và cost đáp ứng; cần benchmark thực tế.

## Wrong / tempting fixes
- Chỉ chờ green health: không kiểm tra data completeness.
- So sánh count: bỏ sót update và delete.
- Retry toàn bộ batch không có revision guard: duplicate/out-of-order có thể làm lùi state.
- Rollback ngay sang v1: không an toàn nếu v1 không còn nhận tombstone mới.

## Production implications
Cần observability cho checkpoint, lag, gaps, per-record mismatches, stale reads, deleted/withdrawn exposure và read-target. Alert phải có runbook và người chịu trách nhiệm.

## Trade-offs
Blue-green cần storage tạm và vận hành thêm một index; đổi lại có thể test đầy đủ trước cutover và rollback có điều kiện. CDC cần xử lý ordering, retention, gap và deduplication. Một thiết kế khác vẫn có thể đúng nếu chứng minh mọi acceptance gate.

## Senior engineer nên nhận ra
Availability và correctness là hai SLO khác nhau. Không thể tuyên bố migration thành công chỉ vì API không lỗi. Đặc biệt với withdrawal, stale data là vấn đề an toàn và quyền riêng tư, không đơn thuần eventual consistency.
