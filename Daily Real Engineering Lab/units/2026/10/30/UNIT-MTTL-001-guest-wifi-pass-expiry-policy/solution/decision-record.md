# ADR tham khảo — Guest Wi-Fi pass authorization

**Status:** Proposed. Đây là một phương án hợp lý theo ràng buộc giả định, không phải đáp án kiến trúc duy nhất.

## Decision

Gateway .NET xác thực mỗi lượt truy cập bằng một truy vấn trạng thái thẻ hiện tại theo ID, kiểm tra expiresAtUtc và revokedAtUtc ở request boundary. MongoDB TTL index chỉ làm cleanup. Để đáp ứng SLA thu hồi 5 giây, ban đầu không cache kết quả ALLOW giữa các gateway; ưu tiên đọc từ primary và đo read-after-write sau khi lệnh thu hồi được xác nhận. Nếu trạng thái không xác định do lỗi DB, không ALLOW; trả lỗi tạm thời có mã riêng để hỗ trợ vận hành.

## Alternatives

| Option | Benefits | Risks / Costs |
|---|---|---|
| Document existence only | Ít logic, rẻ | Không đáp ứng expiry/revocation khi cleanup trễ |
| DB check + explicit validity | Dễ kiểm chứng, một nguồn trạng thái | Thêm read load, phụ thuộc DB và clock discipline |
| Short-lived signed token | Giảm read load, tốt cho edge | Thu hồi trước hạn cần denylist/online check hoặc chấp nhận risk window |
| Cache ALLOW với invalidation | Giảm DB latency | Khó chứng minh propagation <=5s trên nhiều gateway; outage invalidation có thể cho phép sai |

## Rollout

1. Bổ sung query/policy, regression tests cho expiry equality và revocation; không thay đổi schema cũ nếu đã có trường.
2. Canary shadow decision: ghi số lượng quyết định lệch mà không ghi token/PII.
3. Chạy load test 2.000 checks/sec, đo p95, primary availability, clock skew và consistency sau revocation trên cả hai gateway.
4. Bật policy mới theo feature flag, theo dõi DENY và lỗi phụ thuộc; có rollback về bản policy đã kiểm chứng an toàn, không rollback sang existence-only nếu đang có rủi ro cho phép sai.
5. Thử outage MongoDB: gateway fail closed, phản hồi tạm thời rõ ràng, alert theo error budget.

## Risks / open questions

MongoDB read preference, read/write concern, replication lag và khả năng bảo đảm đọc trạng thái mới nhất phải được thử trên deployment thực. Một lần đọc từ primary không tự nó chứng minh SLA 5 giây trong mọi failure mode. Cần xác định chính sách thời gian khi gateway clock skew, expiry ngay sát boundary, quyền truy cập đang duy trì qua session dài và retention/audit của quyết định.

## Why not more infrastructure now?

Không có bằng chứng rằng cache hoặc broker là bắt buộc để đạt 2.000 rps/p95 80 ms. Đo trước, tối ưu sau; chỉ thêm cơ chế phức tạp nếu latency/capacity và SLA security đều được chứng minh.
