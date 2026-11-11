# Reference Solution — Spoiler

## Root cause
Worker đóng cửa sổ bằng processing-time, dùng tập dữ liệu đã đến thay vì event-time đầy đủ. 09:10 chưa nhận R3 measuredAt=09:08:30 level=510 nên phát CLEAR sai. Arrival 09:13 chứng minh cửa sổ trước đó có vượt ngưỡng. Arrival 09:17 là correction 515, không phải phép đo thứ hai; arrival 09:14 là duplicate.

## Invariants
1. Không công bố FINAL CLEAR khi cửa sổ chưa có completeness evidence theo policy đã công bố.
2. Bất kỳ observation hợp lệ >= 500 phải dẫn đến WARNING trong 2 phút kể từ khi nhận, kể cả window cũ.
3. Mọi thay đổi kết luận tạo revision tăng đơn điệu, giữ immutable audit history và phát event bù; không sửa/xóa thông báo cũ.
4. Mỗi (stationId,readingId) chỉ một revision hiệu lực, revision lớn nhất; cùng revision payload khác nhau là conflict cần quarantine.
5. Offline/unknown không đồng nghĩa an toàn.

## Thiết kế B được đề xuất
Ingest append-only raw envelope với eventId, measuredAt, receivedAt và schemaVersion; validate clock skew, quarantine tương lai bất thường; dedupe theo (stationId, readingId, revision) và lưu max revision per reading. Event-time window [09:00,09:10) gồm các mẫu measuredAt >=09:00 và <09:10. Không dùng receivedAt để quyết định membership.

Publish WARNING ngay khi thấy threshold breach, không chờ watermark. CLEAR chỉ ở PROVISIONAL khi thiếu completeness; FINAL CLEAR sau watermark + station health gate. Với lateness 8 phút, watermark minh họa có thể là observed event-time frontier minus 8 phút, nhưng phải có chính sách cho trạm idle; tuyệt đối không cho một trạm ngừng gửi làm kẹt mọi cửa sổ vô hạn. Trạm offline được biểu diễn UNKNOWN/DEGRADED, không giả định complete. Vì có 0.5% quá SLA, watermark không đảm bảo không có late event: mọi late event đi qua reconciliation và có thể phát correction sau FINAL, với audit rõ.

Key output (windowId, alertType) và monotonic revision; outbox transaction ghi state+event; downstream idempotency theo (alertId, revision); replay deterministic từ raw events với tie-breaker revision. Khi phát correction sau FINAL, gửi supersedesRevision và notification channel phù hợp. Bảo toàn audit immutable.

## Timeline expected
- 09:10: không có evidence đủ để FINAL CLEAR; có thể PROVISIONAL UNKNOWN.
- 09:13: R3=510 thuộc window 09:00–09:10; WARNING phát <=09:15.
- 09:14: duplicate không đổi state.
- 09:17: revision 2=515 cập nhật evidence; không tăng count; thông báo correction nếu nội dung thay đổi.
- 09:20: R4=499 late >8 phút so với measuredAt; xử lý reconciliation, ghi late metric, không drop.

## Options
| Option | Benefit | Risk | Verdict |
|---|---|---|---|
| A Processing-time + manual correction | Rẻ, dễ triển khai | Human response, unsafe false CLEAR | Không phù hợp safety |
| B Event-time + watermark + revision | Có latency tốt và audit, chịu late events | State, replay, monitoring phức tạp | Chọn |
| C Hold 25 phút | Dễ lý giải | Vi phạm latency WARNING 2 phút nếu hold toàn bộ; vẫn có late >25 | Không chọn làm policy chung |

## Capacity, ops, recovery
12 * 2 = 24 records/minute, 1440/hour; burst 100x cần queue bounded/backpressure, capacity benchmark trước rollout. 2 engineers: dùng một durable queue/DB sẵn có, tránh cluster streaming mới nếu chi phí vượt $250/tháng. RPO <=5 phút bằng durable raw write + backup; RTO <=30 phút bằng replay checkpoints, diễn tập khôi phục. Alert metrics: event-time lag p50/p95/p99, late-arrival rate, offline stations, UNKNOWN duration, outbox backlog, duplicate ratio, correction latency, replay divergence. Canary shadow-compute so sánh với hệ thống cũ; chặn rollout nếu replay A1–A8 fail. Rollback code không rollback audit/revisions; giữ consumer hiểu revision.

## Requirement change
Trạm sync mỗi 45 phút cần per-station freshness SLA và trạng thái UNKNOWN kéo dài; không được tự nâng lateness toàn cục lên 45 phút rồi gọi đó là an toàn. Phân tách WARNING real-time cho mẫu đã nhận và FINAL CLEAR theo station coverage. Cần product owner phê duyệt degraded safety behavior, cảnh báo operational cho trạm offline, và nêu rõ không thể đảm bảo phát WARNING trong 2 phút từ lúc *đo* nếu trạm chưa truyền; chỉ từ lúc *nhận*.

## Wrong fixes
- Tăng delay toàn cục lên 25 phút: chậm warning và vẫn không có bound tuyệt đối.
- Sort theo receivedAt: không khôi phục event-time ordering.
- Dedupe theo timestamp: có thể xóa mẫu hợp lệ từ trạm khác.
- Upsert đè alert cũ: mất audit và không thông báo cho consumer đã nhận bản trước.

## Senior / Architect judgment
Chứng minh safety invariant bằng trace và replay, không lấy average latency làm tiêu chuẩn duy nhất. Định lượng các giả định về completeness và operational failure; trade-off giữa false clear, late correction, chi phí và sự phức tạp phải được chấp thuận minh bạch.