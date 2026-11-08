# Reference Solution — chỉ đọc sau khi tự làm

## Symptoms
Sau 09:00, các worker vẫn ghi object EU-West và bản ghi S-51 đã ACK trước đó không có trong snapshot EU-Central. API không báo lỗi nhưng residency và RPO đều sai.

## Evidence
Worker cache 15 phút; queue messages không mang routing epoch; snapshot watermark 08:51:30 trước write S-51 08:58; storage inventory có object mới sau cutover. Restart một worker không chứng minh 3 worker còn lại an toàn.

## Root cause
Chuyển API router đơn độc không đồng bộ với các đường ghi bất đồng bộ, không có fencing ở write boundary, và chưa hoàn thành đồng bộ mọi write đã ACK. Đây là lỗi **cutover protocol**, không phải chỉ là cache TTL.

## Phương án tham khảo
Chọn controlled cutover có per-tenant write barrier và checkpoint:
1. Inventory mọi write path và sink, bao gồm retries, workers, scheduler, DB, object store; triển khai kiểm tra authoritative tenant home + epoch ở boundary. Cache được phép đọc nhưng stale epoch không được phép commit.
2. Tạo migration state `PREPARE`; copy baseline và CDC/delta theo committed watermark; verify object checksums và DB FK/manifest.
3. Chuyển `DRAINING`: chặn/queue có giới hạn các write Atlas mới; stop dequeue Atlas, chờ in-flight hoàn tất hoặc fence. Đảm bảo mọi side effect trước cutover được checkpoint và reconcile ở EU-Central.
4. Khi không còn in-flight old-epoch write và destination bắt kịp committed high-watermark, chuyển authoritative state bằng CAS từ `DRAINING(epoch=N)` sang `ACTIVE_EU_CENTRAL(epoch=N+1)`. Các sink từ chối old-epoch writes; workers reload epoch, dedup và replay backlog theo tenant.
5. Re-enable API writes; chỉ ACK khi durable write đã thành công ở vùng hợp lệ; queue ACK sau khi idempotent side effect durable.
6. Giữ old region read-only/quarantined theo chính sách; audit mọi write sau cutover, alert bất thường và reconciliation định kỳ.

**Quan trọng:** fencing phải được enforce tại sink hoặc trusted write service kiểm soát sink; chỉ kiểm tra một lần ở gateway không chặn được worker in-flight. Với PostgreSQL, triển khai bằng tenant epoch và transaction guard có tính atomic cùng business write; với object store, dùng writer service có policy enforcement, tránh direct credentials có thể bypass. Khi không bảo đảm barrier atomic trên mọi sink, duy trì pause cho đến khi hoàn tất draining và credential revocation/permission boundary.

## Why it works
Barrier + durable watermark giữ RPO=0 cho các write đã ACK; fencing ngăn old-epoch commits; idempotency giữ retry an toàn; inventory + reconciliation chứng minh cả DB lẫn objects và queue effects đã hội tụ.

## How to verify
Replay V1–V8. Với V2/V3, yêu cầu chứng cứ sink-level deny hoặc write ở EU-Central; V4/V5 kiểm tra unique eventId và monotonic version; V6 kiểm tra manifest; V7 buộc no-go; V8 chỉ cho fail-closed hoặc fail-forward trong EU-Central.

## Alternative fixes
- **Blue/green tenant slice** với dual-write có journal và reconcile: giảm downtime nhưng phức tạp hơn, không thể coi dual-write là atomic xuyên DB/object store.
- **Long pause + full export/import**: đơn giản về nhất quán, nhưng có thể vượt downtime 5 phút và cần business approval.
- **Region-aware queue partition**: hỗ trợ isolation lâu dài nhưng vẫn cần fence và idempotency.

## Wrong / tempting fixes
- Chỉ giảm worker cache TTL: không chặn in-flight hoặc direct old-region writes.
- Restart tất cả workers: không giải quyết missing snapshot delta hay retry.
- Cho phép failback về EU-West khi EU-Central lỗi: vi phạm residency sau mốc bắt buộc.
- Xem HTTP 201 hoặc queue ACK là bằng chứng cross-store consistency: không đủ.
- Xóa EU-West ngay: phá rollback, audit và có thể mất data chưa reconcile.

## Production implications & trade-offs
Điều kiện no-go phải được automation kiểm tra; đo backlog, high-watermark, old-epoch rejection, per-sink inventory và object checksums. Cần owner xử lý quarantine, quyền truy cập tối thiểu, secret/credential rotation, data-retention approval. RPO=0 và downtime 5 phút có thể xung đột khi backlog lớn: phải dừng cutover thay vì lặng lẽ vi phạm.

## What a Senior / Architect should notice
Residency là invariant xuyên các write paths và các side effects bất đồng bộ, không phải một routing flag. Commit point cần có linearizable authority, observable evidence và failure policy. Kế hoạch rollback sau khi dữ liệu mới đã ở EU-Central phải tôn trọng residency; thường fail-forward hoặc restore trong vùng mới, không tự động quay về vùng cũ.