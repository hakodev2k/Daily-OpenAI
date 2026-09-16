# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms
Lookup trả đúng một order nhưng phạm vi đọc tăng theo số logical partition, khiến RU tăng dù result set không đổi.

## 2. Evidence
Starter báo `PARTITIONS_SCANNED 12`, `ITEMS_RETURNED 1`. Business correctness không phải vấn đề; access scope mới là tín hiệu quan trọng.

## 3. Root cause
Read path biết tenant nhưng không dùng tenant identity để định tuyến operation tới logical partition tương ứng. Predicate đúng vẫn được áp dụng sau khi code đã fan-out qua mọi partition trong mô phỏng.

## 4. Why the fix works
Chọn partition của `tenantId` trước rồi mới tìm `orderId`. Trong Cosmos DB thật, nguyên tắc tương ứng là cung cấp partition-key routing phù hợp cho tenant-scoped operation khi contract cho phép.

## 5. How to verify
`verify.ps1` yêu cầu `PARTITIONS_SCANNED 1`, `ITEMS_RETURNED 1` và vẫn trả `ORD-07-013`.

## 6. Alternative fixes
Nếu lookup chủ yếu theo một key khác và tenant không luôn có sẵn, có thể cần xem lại data model, synthetic partition key hoặc một lookup/index strategy khác. Đó là quyết định modeling, không phải thêm một query option tùy tiện.

## 7. Wrong or misleading fixes
- Tăng provisioned RU: có thể giảm throttling nhưng không sửa access pattern.
- Chỉ thêm predicate `TenantId` nhưng request vẫn fan-out: correctness đúng nhưng routing chưa chắc đã tối ưu.
- Cache mọi lookup: chuyển vấn đề sang consistency/invalidations mà chưa chứng minh cache cần thiết.

## 8. Production implications
Cross-partition query có thể hoàn toàn hợp lệ cho search/reporting. Vấn đề là dùng nó cho read path có sẵn partition identity và chạy với tần suất cao mà không nhận biết cost profile.

## 9. Trade-offs
Partition-scoped read hiệu quả hơn nhưng coupling application contract với partitioning strategy tăng. Nếu partition strategy thay đổi, repository/data-access boundary cần được thiết kế để tránh rò rỉ quá nhiều storage detail.

## 10. What a Senior engineer should notice
Đừng đánh giá query chỉ bằng latency và số record trả về. Với Cosmos DB cần quan sát request charge, partition fan-out, data distribution và access pattern. Một query trả một item vẫn có thể đắt nếu storage phải tìm ở nhiều partition.