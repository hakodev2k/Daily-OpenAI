# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Request trả HTTP 200, starter báo đã index đủ bốn tuyến; search gateway thực tế chỉ lưu `route-101` và `route-104`. `route-102` và `route-103` không xuất hiện.

## Evidence
Trong `evidence/bulk-response-first-attempt.json`, top-level `errors=true` và từng `items[].index.status` lần lượt là 201, 429, 400, 201. Fake gateway giữ `Attempts` và `ActuallyIndexedIds` để kiểm tra independently.

## Root cause
`EnsureSuccessStatusCode()` chỉ xác nhận request HTTP được nhận và xử lý ở cấp request. Bulk API có thể trả HTTP 200 nhưng từng action thất bại. Starter đánh đồng hai tầng trạng thái và công bố thành công không đúng.

## Why the fix works
[Reference code](BulkIngestor.cs) đọc từng `index.status`, đối chiếu `_id` và số lượng item với batch đã gửi; chỉ retry các action tạm thời lỗi 429/5xx, cách ly action 400, giữ kết quả 2xx. Sau tối đa ba lần, các lỗi transient chưa giải quyết nằm trong `RetryExhaustedIds` thay vì bị coi là thành công. Fake gateway làm `route-102` thành công ở lần thứ hai.

## How to verify
Chạy `./verify.ps1` sau khi sửa `starter/BulkIngestor.cs`. Để tự kiểm tra reference, chạy:
```powershell
dotnet run --project ./solution/BulkLab.Solution.csproj --configuration Release
```
Kỳ vọng `CHECK_PASSED`, `INDEXED_ACTUAL=route-101,route-102,route-104`, `QUARANTINED=route-103`, `route-102:2` và các route khác một lần. `verify.ps1` **không** dùng reference project.

## Alternative fixes
Có thể dùng official Elasticsearch .NET client để phân tích item response, hoặc tự đọc JSON như bài này. Trong production cần xác minh version của client và API contract. Với workload nhỏ, xử lý batch đồng bộ có kiểm soát là đủ; workload lớn cần durable retry queue và DLQ.

## Wrong / tempting fixes
- Retry cả batch: gửi lại 201, có thể tăng write amplification và không giải quyết lỗi mapping 400.
- Chỉ kiểm tra HTTP status hoặc top-level `errors`: không biết item nào cần retry hay cách ly.
- Bỏ qua 400 và báo thành công: làm sai lệch số liệu và mất khả năng sửa dữ liệu.
- Retry vô hạn hoặc retry tức thì trong production: có thể khuếch đại overload. Simulator không mô phỏng delay; hệ thật cần exponential backoff, jitter và retry budget.

## Production implications
Bulk request Elasticsearch thường dùng NDJSON và có thể chứa nhiều operation type. Không giả định mọi response luôn cùng thứ tự nếu adapter thực tế thay đổi; phải đối chiếu action identity. Persist kết quả failed/permanent vào DLQ với correlation ID, giới hạn log payload nhạy cảm, xử lý cancellation, circuit breaking, metrics theo status và reconciliation từ nguồn dữ liệu gốc. Thử với Elasticsearch thật ở môi trường riêng trước triển khai.

## Trade-offs
Retry inline đơn giản nhưng kéo dài batch latency và mất tiến độ nếu process crash. Durable queue tăng độ bền nhưng tốn vận hành, đòi hỏi idempotent document IDs và observability. Fail-fast cả batch tránh báo thành công giả nhưng làm giảm khả năng phục hồi của item hợp lệ.

## What a Senior engineer should notice
Định nghĩa success phải dựa trên business invariant và per-item outcome; phân biệt permanent vs transient, tránh retry thành công, có audit/reconciliation, và xem xét crash giữa indexing và ghi checkpoint.
