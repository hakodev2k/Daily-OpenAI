# Scenario — Customs clearance event intake

## Kiến trúc hiện tại
```text
Partner customs gateway
  -> HTTPS webhook (keyId, timestamp, eventId, HMAC-SHA256)
  -> 3 ASP.NET Core replicas
  -> durable inbox (unique partnerId + eventId)
  -> parcel state transition
```

Các replica đọc cấu hình khóa từ cùng một secret store. Signature bao phủ `timestamp + "." + rawBody` theo hợp đồng đối tác; không ký lại JSON đã deserialize. Webhook có thể retry. Một event được xử lý business đúng một lần theo `(partnerId, eventId)`; nếu cùng ID nhưng body hash khác, phải cách ly và cảnh báo.

## Mục tiêu nghiệp vụ
- Event hợp lệ do đối tác phát hành phải được phản ánh trong parcel ledger **không quá 24 giờ kể từ createdAt**, kể cả khi lần giao đầu thất bại.
- Không phát hành trạng thái `Released` từ payload không được xác thực.
- Sự kiện trùng lặp không được tạo hai business transitions.
- Độ trễ bình thường p95 mục tiêu dưới 2 phút; yêu cầu 24 giờ là hard recovery bound, không phải p95.

## Mốc rollout — 2026-10-30 (+07)
- 09:00: đối tác bắt đầu dùng `K2` cho event mới.
- 09:12: đối tác xác nhận mọi event **mới** đã dùng `K2`.
- 09:30: Security yêu cầu **không xác thực bất kỳ payload nào bằng K1** kể từ mốc này.
- Retry có thể kéo dài đến 24 giờ sau `createdAt`, và **giữ nguyên body, timestamp, keyId, signature ban đầu**.
- K2 được cấp trước 09:00; không có outage của K2 trong kịch bản cơ sở.

## Dữ liệu và phạm vi
Dùng evidence/delivery-events.csv như một tập sự kiện synthetic cố định. Cột `httpObserved` mô tả kết quả của pilot webhook-only với hard cutoff; không phải toàn bộ business processing. Đối tác có thêm một **authoritative pull feed** đọc qua TLS + OAuth2 client credentials, trả eventId/body/createdAt/cursor, retention 48 giờ, cập nhật thông thường trong 2 phút; **chưa có chứng cứ kiểm thử completeness**. Đừng coi thông số quảng cáo là SLA đã chứng minh.

## Quyết định cần đưa ra
1. Hệ thống có thể đáp ứng đồng thời hard cutoff và 24h recovery nếu **chỉ dùng webhook** không?
2. Đề xuất phương án A: giữ K1 đến hết retry window; B: thu hồi đúng giờ và chấp nhận mất event; C: hard cutoff + authenticated reconciliation; D: đàm phán đối tác ký lại retry bằng K2. So sánh feasibility, risk, operational cost, proof.
3. Thiết kế state machine, inbox idempotency, xác thực feed, monitoring, backfill và rollback. Không lưu K1 vô thời hạn.
4. Ghi rõ điều kiện GO/NO-GO; đừng gọi một phương án “đạt” khi chưa kiểm chứng feed completeness và end-to-end latency.

## Bằng chứng cần thu
Event-by-event acceptance matrix, late-first-delivery count, key validation decisions, deduplication results, reconciliation coverage, gap detection, alert thresholds, owner và test plan.

## Giới hạn
Không dùng secret thật. Không yêu cầu triển khai code production. Đây là design exercise, không phải hướng dẫn bypass xác thực.
