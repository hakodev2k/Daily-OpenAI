# Reference Solution — SPOILER

**Chỉ xem sau khi tự chạy mô hình và viết ADR.** Đây là một phương án có thể bảo vệ được dưới các ràng buộc đã cho, không phải kiến trúc duy nhất đúng.

## 1. Symptoms

Thay mapping/index ngay trên đường query đã khiến p95 tăng từ 180 lên 470 ms, 5xx lên 2.1%, freshness lag 22 phút và 9/10,000 listing trong sample bị thiếu. PostgreSQL vẫn hoạt động bình thường.

## 2. Evidence

18 triệu listing, backfill 900 docs/s, nguồn thay đổi 350 events/s, replay 500 events/s, CDC retention 72 giờ. Storage v1 58 GB, v2 72 GB, tổng 130 GB < 140 GB (chưa tính headroom cho segment merge; phải đo thêm). Peak query 1,800/s.

## 3. Root cause / decision pressure

Một schema migration dài nhiều giờ không thể được xem như thao tác thay thế đồng bộ trên read model đang phục vụ traffic. Cần có ranh giới snapshot và chuỗi thay đổi để v2 phản ánh trạng thái nguồn nhất quán. Lần thử trước không có cutover gate/rollback có thể kiểm chứng; timeline chưa đủ để kết luận một dòng code cụ thể gây ra sự cố.

## 4. Phương án đề xuất

```text
PostgreSQL (source of truth)
   | snapshot at durable CDC position P0
   +-------------------> bulk backfill --> search_v2
   |                                          ^
   +--> durable CDC (entityId, version, tombstone) --> replay worker
                                                  |
Clients -> query API (v1/v2-compatible) -> read alias -> search_v1 OR search_v2
```

- Tạo `search_v2` riêng với mapping và analyzer đã version hóa; `search_v1` tiếp tục phục vụ query.
- Lấy durable CDC position P0 gắn với snapshot nhất quán, backfill từ snapshot đó. Nếu snapshot không thể nhất quán, thiết kế checkpoint và version guard để ngăn dữ liệu cũ ghi đè mới.
- Sau backfill, replay từ P0; consumer idempotent theo `(entityId, version)`, bỏ qua version cũ hơn, áp dụng tombstone cho delete.
- Giữ query API có khả năng đọc v1/v2 qua feature flag; shadow queries và so sánh semantic results, quyền hiển thị, latency, freshness.
- Canary 1% → 5% → 25% → 100%, mỗi bước có tối thiểu 30 phút telemetry. Đổi read alias atomically sau go/no-go; nếu application contract không tương thích thì deploy code hỗ trợ cả hai schema trước.
- Giữ v1 nhận thay đổi trong rollback window hoặc duy trì khả năng rebuild/replay v1 từ durable log; không giả định index cũ còn fresh sau khi ngừng cập nhật.
- Rollback: đưa read alias và flag về v1 trong <10 phút sau khi xác nhận v1 đủ mới, đồng thời duy trì event replay cho v2. Chỉ xóa v1 sau khi qua cửa sổ rollback.

## 5. Tính toán và verify

- Backfill = 18,000,000 / 900 = 20,000 giây ≈ **5.56 giờ**.
- Backlog sau backfill ≈ 350 × 20,000 = **7,000,000 events**.
- Net catch-up = 500 − 350 = **150 events/s**.
- Catch-up ≈ 7,000,000 / 150 = 46,667 giây ≈ **12.96 giờ**.
- Tổng ≈ **18.52 giờ** (chưa tính validate/canary); nằm trong CDC retention 72h, nhưng phải xét headroom cho 48h budget.
- Peak raw index footprint ≈ 58 + 72 = **130 GB**; chỉ còn 10 GB headroom, cần đo segment merges, replicas và disk watermarks; nếu vượt thì **no-go** cho đến khi tăng capacity/giảm footprint.
- Trước cutover: replay lag <= 90s liên tục 30 phút, semantic parity theo sample phân tầng, delete/tombstone correctness, tenant visibility, p95 <=250ms ở 1,800 qps, canary không tăng 5xx và rollback drill thành công.
- Sau cutover: quan sát ít nhất một rollback window và xác nhận v1 vẫn đủ mới.

## 6. Requirement change — campaign 600 events/s

Replay 500/s < 600/s nên backlog **tăng** 100 events/s trong surge; không có finite catch-up trong điều kiện surge kéo dài. Trong 4 giờ backlog tăng thêm ~1.44 triệu events nếu worker đang replay liên tục. Nếu không thể chứng minh tăng replay capacity hoặc kéo dài window an toàn, hoãn cutover cho đến khi surge kết thúc và backlog quay về ngưỡng; kiểm tra lại retention, storage và budget. Không được tự tuyên bố "scale out" nếu không có load test và nguồn lực.

## 7. Alternatives

| Phương án | Điểm mạnh | Điểm yếu / điều kiện |
|---|---|---|
| In-place reindex trên v1 | Ít storage tạm | Khó bảo vệ availability, rollback và mapping compatibility; không phù hợp SLO hiện tại |
| Versioned v2 + CDC + alias (đề xuất) | Có đường backfill tách biệt, canary và rollback rõ | Tốn storage, replay phức tạp, cần version/tombstone và dual-schema compatibility |
| Dual write trực tiếp từ API | Giảm lag lý thuyết | Coupling write latency với search, partial failure và consistency phức tạp; chỉ chấp nhận với outbox/repair có kiểm chứng |

## 8. Wrong / tempting fixes

- **Chỉ so document count**: không chứng minh field, delete, quyền truy cập hoặc kết quả truy vấn đúng.
- **Dừng traffic vài phút**: backfill tính bằng giờ, không đáp ứng yêu cầu availability.
- **Tăng số worker vô hạn**: có thể phá search p95, storage watermarks và chi phí; cần load test capacity.
- **Đổi alias ngay khi backfill xong**: bỏ qua backlog cập nhật phát sinh trong lúc copy.
- **Xóa v1 ngay sau cutover**: phá rollback <10 phút.
- **Tin rằng CDC exactly-once**: phải thiết kế idempotency/version guard khi redelivery hoặc reorder.

## 9. Production implications / trade-offs

Ghi lại checkpoint P0, tiến độ backfill, replay lag, tỉ lệ conflict/out-of-order, tombstone count, p95 theo nhóm query, shard disk watermark, query correctness và rollback status. Chỉ giữ hai index trong window được duyệt; vận hành có quyền no-go khi storage không đủ. Data security phải được kiểm chứng qua tenant visibility trên cả v1 và v2.

## 10. What a Senior engineer should notice

Bài toán không phải "dùng alias hay không" mà là **chứng minh một read model mới có thể thay read model cũ** trong giới hạn freshness, availability, capacity và rollback. Một quyết định tốt định nghĩa rõ invariant, evidence, owner, go/no-go và hậu quả của requirement change; không chọn công nghệ chỉ vì nghe có vẻ hiện đại.
