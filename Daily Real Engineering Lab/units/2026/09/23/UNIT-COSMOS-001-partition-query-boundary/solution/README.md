# Reference Solution — Spoiler

## 1. Symptoms
Tenant-scoped read trả đúng 20 records nhưng chạm cả 8 logical partitions, scan 160 items và có request-unit cost cao hơn cần thiết.

## 2. Evidence
Diagnostics của simulator tách correctness khỏi execution scope: records đúng nhưng `partitionsTouched`, `scanned` và `requestUnits` tăng theo số partition.

## 3. Root cause
Caller biết partition key (`tenantId`) nhưng không truyền routing information vào query boundary. Filter vẫn loại records sai tenant, nhưng engine phải fan out rồi mới filter.

## 4. Why fix works
Truyền known partition key giới hạn execution vào đúng logical partition trước khi filter, nên correctness không đổi nhưng fan-out biến mất.

## 5. How verify
Sửa learner path để `Query(..., partitionKey: targetTenant)`, chạy `verify.ps1`; phải thấy 20 records, 1 partition và 20 scanned items.

## 6. Alternative fixes
Nếu access pattern thực sự cần cross-partition query, có thể chấp nhận fan-out và tối ưu model/index/query tùy workload. Với read đã biết tenant, explicit partition routing thường rõ ràng hơn.

## 7. Wrong or misleading fixes
- Thêm cache ngay: có thể che latency nhưng không sửa query scope và tăng consistency complexity.
- Scale throughput ngay: trả thêm RU cho công việc dư thừa.
- Chỉ giữ `WHERE TenantId = ...`: filter đúng dữ liệu nhưng không nhất thiết cung cấp routing boundary.

## 8. Production implications
Theo dõi request charge, diagnostics và partition fan-out. Partition key phải phù hợp access patterns; hot partition và cross-partition analytics là bài toán khác.

## 9. Trade-offs
Routing theo partition key tối ưu tenant-scoped reads nhưng partition strategy ảnh hưởng distribution, transactional scope và các query toàn cục.

## 10. What a Senior engineer should notice
Correct result không đồng nghĩa efficient execution. Với distributed database, cần phân biệt logical predicate, physical routing và cost model; đo diagnostics trước khi thêm infrastructure.