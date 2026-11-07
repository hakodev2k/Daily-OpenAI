# Architecture decision worksheet — tự hoàn thành trước khi xem solution

## Hợp đồng nghiệp vụ
- Sự kiện ở đây là **full snapshot** (không phải delta), có `SiteId`, `Version`, `EventId`.
- Bản mới hơn phải thắng bản cũ hơn cho cùng một SiteId.
- Cùng EventId, cùng payload không tạo side effect lặp.
- Cùng EventId nhưng payload khác, hoặc cùng SiteId/Version nhưng payload khác, cần phát tín hiệu conflict để điều tra.
- Event của SiteId khác phải độc lập.

## Constraints
- 1.5 triệu delivery/ngày, có redrive và duplicate; tối đa 4 consumer instances.
- Dashboard đọc theo SiteId, p95 mục tiêu dưới 150 ms trong điều kiện bình thường.
- Không được đánh dấu broker ack trước khi projection commit bền vững.
- Không được mất audit trail; mọi conflict cần được quarantine, không được âm thầm ghi đè.
- Không cần đồng bộ toàn cầu giữa các vùng trong lab; xác định rõ ranh giới giả định.

## ADR cần nộp
1. So sánh last-delivery-wins, per-site version compare-and-set, và single ordered partition.
2. Chỉ ra **transaction boundary** giữa inbox (EventId), current projection và audit append.
3. Nếu consumer crash sau DB commit nhưng trước broker ack, xử lý thế nào?
4. Nếu version 3 đến trước version 2, có được áp dụng ngay không? Giải thích sự khác biệt full snapshot vs delta.
5. Thiết kế chỉ số `stale_replay_total`, `duplicate_delivery_total`, `conflict_total`, `projection_lag`; lưu ý cardinality.
6. Rollout / backfill / rollback không làm trạng thái công khai thoái lui như thế nào?
7. Điều gì thay đổi nếu nhà cung cấp chuyển từ full snapshot sang delta events?

Viết lựa chọn, bằng chứng, rủi ro tồn dư và điều kiện cần thử nghiệm; không chỉ liệt kê tên pattern.
