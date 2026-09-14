# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Lookup trả đúng invoice nhưng một request đơn lẻ phải chạm nhiều logical partition. Khi số tenant tăng, simulated request units và work performed tăng theo.

## 2. Evidence

Starter in ra `PARTITIONS_TOUCHED` lớn hơn 1 dù request đã có cả `tenantId` và `invoiceId`.

## 3. Root cause

Luồng đọc dùng một cross-partition search theo document id rồi mới lọc theo tenant. Trong mô hình partitioned document store, request đã biết partition key nhưng không dùng nó để route trực tiếp tới logical partition chứa document.

## 4. Why the fix works

Dùng point-read semantics với cả `tenantId` và `invoiceId` giúp storage layer xác định partition ngay từ đầu. Không cần scan các partition khác.

Trong simulator, thay:

```csharp
var result = store.QueryByInvoiceId(tenantId, invoiceId);
```

bằng:

```csharp
var result = store.ReadItem(tenantId, invoiceId);
```

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Kỳ vọng:

```text
FOUND_INVOICE=invoice-03
FOUND_TENANT=tenant-17
PARTITIONS_TOUCHED=1
SIMULATED_REQUEST_UNITS=1
VERIFICATION=PASS
```

## 6. Alternative fixes

Nếu use case thực sự là truy vấn nhiều document, query có partition-key scope vẫn hợp lý. Nếu caller chưa biết partition key, có thể cần thay đổi API contract, routing index hoặc data model thay vì giả vờ rằng mọi lookup đều có thể là point read.

## 7. Wrong / Tempting Fixes

- Tăng throughput/RU ngay: chỉ che chi phí do access pattern chưa tối ưu.
- Cache mọi invoice: tăng consistency và invalidation complexity trong khi request đã có đủ routing data.
- Parallelize scan các partition: có thể giảm wall-clock latency trong simulator nhưng vẫn fan-out và vẫn tiêu tốn work.
- Chỉ thêm index: indexing không biến cross-partition lookup thành point read.

## 8. Production implications

Trong Cosmos DB, data model và partition key là một phần của API access contract. Một endpoint thường xuyên đọc một item cụ thể nên ưu tiên access pattern có thể định tuyến trực tiếp khi business key đã cung cấp partition identity.

## 9. Trade-offs

Point read hiệu quả nhưng yêu cầu caller biết đúng partition key. Nếu partition key không tự nhiên xuất hiện trong request, việc thêm lookup table hoặc redesign key có thể tạo complexity khác và phải đánh giá theo workload thực tế.

## 10. What a Senior engineer should notice

Senior engineer không chỉ nhìn latency hiện tại mà nhìn growth function: một lookup O(number of partitions) sẽ trở thành vấn đề vận hành khi tenant count tăng. Cần xem access pattern, partition key và API boundary như một thiết kế thống nhất thay vì tối ưu từng query rời rạc.
