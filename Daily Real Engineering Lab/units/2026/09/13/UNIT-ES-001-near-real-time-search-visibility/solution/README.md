# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Write trả success, direct GET theo document ID trả `200`, nhưng `_search` ngay sau write trả `0` hit. Sau khi chờ qua refresh interval, search trả `1` hit.

## 2. Evidence

Ba evidence quan trọng:

- write đã được acknowledge
- realtime document GET có thể đọc được document
- search visibility chỉ xuất hiện sau refresh boundary

Điều này cho thấy dữ liệu không bị mất và request body không sai; vấn đề nằm ở visibility semantics của search.

## 3. Root cause

Elasticsearch search là near-real-time. Index request hoàn thành không mặc định có nghĩa document đã trở thành searchable trong search segments. Starter cố tình đặt `index.refresh_interval` thành `30s` và gửi write với `refresh=false`, nên direct GET có thể thấy document trước `_search`.

## 4. Why the fix works

Với workflow cụ thể này, business requirement yêu cầu publish xong thì preview bằng search phải thấy document trước khi trả success. Một fix phù hợp là dùng write refresh policy `wait_for` cho boundary này:

```csharp
var refreshPolicy = "wait_for";
```

Request write sẽ chờ refresh tiếp theo làm thay đổi trở nên searchable, thay vì ép refresh ngay lập tức cho toàn index.

## 5. How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kỳ vọng:

- write thành công
- direct GET thành công
- `IMMEDIATE_SEARCH_HITS=1`
- `DELAYED_SEARCH_HITS=1`

## 6. Alternative fixes

- Thiết kế UI chấp nhận eventual consistency và poll có bounded timeout nếu business không thật sự cần immediate visibility.
- Preview bằng source-of-truth/database thay vì search index nếu search chỉ là derived read model.
- Explicit `_refresh` phù hợp cho maintenance/test workflow hiếm, nhưng không nên dùng mặc định sau từng document write.

## 7. Wrong or misleading fixes

### `Task.Delay` cố định

Có thể che triệu chứng trên máy hiện tại nhưng không tạo consistency contract. Refresh interval, load và cluster state có thể thay đổi.

### Gọi `_refresh` sau mỗi write

Có thể làm bài test pass nhưng ép tạo searchable segments quá thường xuyên, làm giảm indexing throughput và tăng resource cost.

### Retry write nhiều lần

Write đã thành công; retry không giải quyết search visibility và còn có thể tạo side effect nếu ID/operation không idempotent.

## 8. Production implications

Read-after-write phải được định nghĩa theo từng workflow. Không nên biến một yêu cầu preview của editor thành global consistency policy cho mọi indexing traffic.

## 9. Trade-offs

`refresh=wait_for` tăng write latency tới refresh tiếp theo nhưng tránh forced refresh per write. Nếu workload có throughput cao và không cần immediate search visibility, `refresh=false` cùng eventual consistency thường phù hợp hơn.

## 10. What a Senior engineer should notice

- "Write acknowledged" và "search-visible" là hai contract khác nhau.
- Cần xác định source of truth và consistency requirement trước khi chọn kỹ thuật.
- Fix đúng thường nằm ở workflow boundary, không phải global cluster tuning.
- Test nên kiểm tra observable contract thay vì ngủ một khoảng thời gian rồi hy vọng.
