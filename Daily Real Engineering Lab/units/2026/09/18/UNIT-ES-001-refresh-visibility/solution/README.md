# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Write được acknowledge nhưng immediate search chưa trả document; sau visibility cycle thì cùng query trả đúng dữ liệu.

## 2. Evidence
Direct lookup trong simulator thấy committed document trong khi search snapshot chưa thấy nó. Điều này loại trừ giả thuyết write bị mất.

## 3. Root cause
Workflow giả định search index cung cấp immediate read-after-write visibility. Elasticsearch search là near-real-time: write acknowledgement và khả năng nhìn thấy document qua search không phải cùng một consistency boundary.

## 4. Why the fix works
Workflow cần xác định rõ visibility contract. Với simulator, thêm `index.Refresh()` trước search biểu diễn targeted refresh boundary cho workflow thực sự yêu cầu immediate search visibility:

```csharp
index.Index(ticket);
index.Refresh();
return index.Search(ticket.Id);
```

Trong Elasticsearch thật, lựa chọn tương ứng có thể là request-specific refresh behavior như `refresh=wait_for` khi business flow thực sự cần chờ visibility, thay vì ép refresh toàn hệ thống sau mọi write.

## 5. How to verify
Chạy `./verify.ps1`; immediate result phải chứa ticket vừa tạo và không dùng arbitrary sleep.

## 6. Alternative fixes
Nếu UI chỉ cần hiển thị entity vừa tạo, đọc từ system of record hoặc trả representation từ create API có thể tốt hơn việc search ngay. Nếu eventual visibility được chấp nhận, client có thể thiết kế UX phù hợp thay vì ép synchronous visibility.

## 7. Wrong or misleading fixes
`Start-Sleep`/`Task.Delay` với số giây tùy ý chỉ làm race ít xuất hiện hơn. Force refresh sau mọi write có thể tăng indexing/search cost. Retry vô hạn che giấu consistency contract và làm latency khó dự đoán.

## 8. Production implications
Cần phân biệt durability/acknowledgement, GET visibility và search visibility. Automation tests thường làm lộ assumption này vì create và search chạy sát nhau hơn thao tác người dùng.

## 9. Trade-offs
Immediate visibility tăng tính đơn giản cho một số workflow nhưng có chi phí throughput/latency. Near-real-time behavior thường phù hợp search workloads hơn nếu product flow không đòi strong immediate visibility.

## 10. What a Senior engineer should notice
Không nên sửa search engine trước khi xác định business consistency requirement. Hãy chọn contract theo workflow, đo chi phí, và tránh biến một yêu cầu cục bộ thành global refresh policy.