# Reference Solution — spoiler

## 1. Symptoms & evidence
T1 không thể đặt NOT NULL vì 2.1M row NULL. T3 có thể làm qua kiểm tra null nhưng phá vỡ ý nghĩa dữ liệu; T4/T5 cho thấy không tương thích giữa DDL và writer cũ. CPU thấp không chứng minh migration không gây lock.

## 2. Root cause
Thay đổi schema bắt buộc trước khi toàn bộ writer và dữ liệu lịch sử đạt hợp đồng mới. Rollout API không bao phủ offline worker; default sentinel chỉ che giấu thiếu dữ liệu.

## 3. Phương án khuyến nghị
**Expand:** giữ `station_zone` nullable, thêm logic API v2/worker v2 ghi zone hợp lệ và telemetry về writer version. Không dùng sentinel làm zone hợp lệ. Nếu worker v1 còn hoạt động, duy trì compatibility trong giai đoạn chuyển tiếp; có thể enrich phía server từ nguồn mapping đã xác thực, nhưng phải quarantine nếu không thể xác minh.

**Backfill:** batch nhỏ, idempotent, chỉ cập nhật `WHERE station_zone IS NULL` và mapping có provenance. Row thuộc trạm đã ngừng hoạt động phải phục hồi mapping lịch sử từ nguồn đáng tin cậy hoặc quarantine để xử lý; không bịa zone. Backfill cần checkpoint, throttle, đo WAL/replica lag, retry an toàn.

**Compatibility gate:** dừng đường ghi v1 sau khi mọi producer được nâng cấp và offline backlog tối đa 48 giờ đã được xử lý; xác nhận không còn insert thiếu zone qua quan sát thực tế. Nếu cần hỗ trợ v1 dài hơn, giữ schema nullable hoặc thiết kế server-side compatibility path đã xác thực.

**Contract:** sau khi đã xử lý row lịch sử và mọi writer tương thích, thêm constraint có kiểm soát. Ví dụ:
```sql
ALTER TABLE dock_events
  ADD CONSTRAINT dock_events_zone_present
  CHECK (station_zone IS NOT NULL) NOT VALID;
ALTER TABLE dock_events VALIDATE CONSTRAINT dock_events_zone_present;
```
Lưu ý: `NOT VALID` vẫn kiểm tra **new writes** ngay khi thêm constraint; vì thế không thực hiện bước này khi còn writer cũ gửi NULL. `VALIDATE` quét dữ liệu lịch sử với đặc tính khóa khác so với thay đổi trực tiếp; vẫn phải đo lock/lag trên staging. Sau khi validated, cân nhắc `SET NOT NULL` trong maintenance window kiểm soát timeout, kiểm tra version PostgreSQL và execution plan của migration; có thể giữ validated CHECK nếu chính sách schema cho phép.

**Rollback:** trước contract, rollback API dễ hơn. Sau contract, rollback sang v1 không an toàn; chỉ rollback về phiên bản tương thích hoặc thay đổi schema có kiểm soát và đánh giá lại integrity. Không tự động DROP constraint chỉ để làm xanh dashboard.

## 4. Why this works
Tách thay đổi dữ liệu, writer compatibility và schema enforcement; mỗi pha có exit criteria đo được, không có thời điểm bắt buộc dữ liệu chưa thể cung cấp.

## 5. Verify
Dùng G1–G8 trong `docs/acceptance-matrix.md`; kiểm thử old/new writers, replay offline, rollback và backfill idempotency trên staging. Truy vấn mẫu:
```sql
SELECT count(*) FROM dock_events WHERE station_zone IS NULL;
SELECT station_id, count(*) FROM dock_events
WHERE station_zone IS NULL GROUP BY station_id ORDER BY count(*) DESC LIMIT 20;
```
Chỉ báo cáo production verified khi có kết quả thực chạy.

## 6. Alternatives and trade-offs
- **Giữ nullable + enforce ở application:** triển khai nhanh, rollback dễ, nhưng rủi ro đường ghi ngoài ứng dụng và tính nhất quán yếu hơn.
- **Server-side enrichment trong transition:** hỗ trợ writer cũ, nhưng phụ thuộc mapping lịch sử và độ tin cậy nguồn; lỗi mapping cần quarantine.
- **Bảng mới + dual-write + cutover:** hợp lý khi cần đổi nhiều thuộc tính cùng lúc, nhưng tăng chi phí đối soát, drift và rollback.
- **Expand/backfill/contract:** ít thay đổi hơn, phù hợp với một trường bắt buộc và rollout nhiều producer.

## 7. Wrong / tempting fixes
- `DEFAULT 'UNKNOWN'`: thỏa NOT NULL về mặt cú pháp nhưng không đảm bảo dữ liệu đúng.
- Chỉ nâng cấp API: bỏ sót worker và delayed messages.
- Chạy `SET NOT NULL` ngay sau backfill: vẫn có cửa sổ new writes NULL nếu old writer còn hoạt động.
- Backfill một transaction lớn: rủi ro lock/WAL/bloat, khó rollback và kiểm soát tiến độ.

## 8. Production implications
Phải có dashboard insert errors theo writer/version, backfill progress, null count, quarantine count, lock wait và replica lag. Đặt `lock_timeout`, rollout có gate và on-call owner.

## 9. Senior engineer checklist
Không chỉ hỏi migration SQL có chạy hay không; phải chứng minh tính tương thích của **mọi producer**, tính xác thực của dữ liệu lịch sử, recovery path, và điều kiện chuyển pha dựa trên evidence.