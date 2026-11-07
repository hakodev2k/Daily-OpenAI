# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, thử policy và chạy verify. Đây là **một phương án bảo vệ được** dưới ràng buộc scenario, không phải một khẳng định rằng mọi hệ thống Cosmos DB phải dùng 8 shard.

## 1. Symptoms

Hợp tác xã lớn bị throttling trong khung phát giống; tổng provisioned throughput có thể chưa sử dụng hết. Dashboard không phản ánh ngay toàn bộ redemption.

## 2. Evidence

Policy `cooperative` gom 900/1.000 voucher vào một logical bucket. Tổng số bucket hoặc tổng RU không nói lên phân phối tải. Trên môi trường thật cần đối chiếu normalized RU consumption, 429, hot logical partition keys, p95 và query metrics.

## 3. Root cause

Partition key gắn trực tiếp với cooperative ID; phân phối lưu lượng không đồng đều. Một hợp tác xã áp đảo có thể tạo hotspot ngay cả khi các partition khác rảnh. Đây là vấn đề phân phối và data access contract, không chỉ là tổng capacity.

## 4. Why the fix works

Đặt `starter/policy.json` thành:

```json
{ "partitionStrategy": "voucher-shard" }
```

Simulator dùng SHA-256 của voucher ID và lấy byte đầu modulo 8, nên cùng ID luôn vào cùng shard, độc lập cooperative hoặc attempt. Mỗi voucher được phân tán trên 8 logical bucket synthetic. `Get(voucherId)` tự suy ra shard, không cần lookup. Trong Cosmos DB thật, partition key value và item ID phải được lưu nhất quán; unique ID chỉ được đảm bảo trong phạm vi logical partition. Mọi attempt cho cùng voucher phải cùng partition và dùng **conditional create / atomic write** để tránh đổi trùng; không coi việc chỉ hash ID là đủ cho idempotency.

## 5. How to verify

`pwsh -NoProfile -File ./verify.ps1` chạy **learner-edited** `starter/policy.json`, không chạy reference policy. Các checks gồm hotspot, retry stability và point-read derivability. Sau đó review kế hoạch migration và contract idempotency trong workspace.

## 6. Alternative fixes

- Giữ cooperative key và thêm capacity có thể hữu ích khi phân phối chưa chạm giới hạn hot partition; không bảo đảm giải quyết skew nặng.
- Chia shard theo voucher ID nhưng dùng nhiều hơn 8 giá trị nếu benchmark cho thấy cần; cân bằng số partition key, truy vấn và vận hành.
- Một mapping/index lookup theo voucher ID có thể cho phép chiến lược khác nhưng vi phạm yêu cầu không thêm lookup đồng bộ trong bài này.
- Một read model theo cooperative/day cập nhật bất đồng bộ giúp dashboard tránh fan-out quá lớn, chấp nhận độ trễ tối đa 30 giây.

## 7. Wrong / tempting fixes

- Tăng tổng RU/s vô điều kiện: có thể không giải quyết giới hạn partition nóng.
- Random shard mỗi attempt: cân bằng bucket nhưng retry có thể ghi vào partition khác và làm yếu unique redemption.
- Dùng `cooperativeId + day`: vẫn gom tất cả lượt phát trong cùng ngày/điểm.
- Dựa vào Redis lock để chặn duplicate: lock có thể hết hạn hoặc mất khi crash; cần ràng buộc authoritative.
- Khẳng định 8 bucket synthetic tương ứng 8 physical partitions Cosmos DB: **sai**; mapping và capacity phải được đo thực.

## 8. Production implications

Thiết kế write model có item `id = voucherId`, `partitionKey = StableShard(voucherId)`; write lần đầu phải atomic và có response idempotent cho retry. Nếu nghiệp vụ cho phép thay đổi trạng thái, dùng ETag/conditional update hoặc transactional batch **trong cùng logical partition**; nếu cần invariant cross-partition, phải thiết kế lại boundary, không giả định transaction cross-partition. Tránh tự viết một hàm hash khác ở các service; version hóa thuật toán và không thay đổi mapping của voucher đã lưu.

## 9. Trade-offs / migration

Hash shard giảm skew nhưng dashboard theo cooperative/day trở thành cross-partition query. Dùng change feed/read model, theo dõi lag và reconciliation; dashboard không phải nguồn xác nhận redemption. Migration 6 tuần: (1) instrument baseline, (2) dual-read với kiểm tra equivalence và backfill có checkpoint, (3) shadow validate, (4) cutover writes có gate/idempotency, (5) reconcile và rollback plan. Không dual-write thiếu cơ chế chống duplicate. Với Cosmos DB, thay partition key của container hiện hữu thường cần container mới và migration; xác minh đặc tính tài khoản và SDK đang dùng.

## 10. What a Senior engineer should notice

Chỉ số simulator là **logical event-count skew**, không mô phỏng request units, physical partition split, query RU, latency hay session consistency. Bằng chứng quyết định phải gồm production-like load test, per-partition metrics, failure injection, retry/idempotency tests, read model lag và kế hoạch rollback. Tính đúng đắn của redemption quan trọng hơn việc giảm 429.

## Kết luận

Chọn stable voucher-derived shard khi access pattern chủ đạo là voucher-ID point read và idempotent redemption. Nếu phần lớn query thực tế yêu cầu giao dịch theo cooperative hoặc dữ liệu cần invariant ở cấp cooperative, đánh giá lại aggregate boundary trước khi triển khai.
