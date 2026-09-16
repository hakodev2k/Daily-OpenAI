# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Hai dependency calls riêng lẻ hoàn thành, nhưng tổng operation vượt business deadline 900ms.

## 2. Evidence
Timeline cho thấy call thứ hai bắt đầu sau khi call thứ nhất đã tiêu thụ phần lớn budget; không có cancellation scope đại diện cho deadline tổng.

## 3. Root cause
Code chỉ truyền `requestAborted`. `BusinessBudget` tồn tại như dữ liệu nhưng không được biến thành một deadline/cancellation policy cho aggregate operation. Vì vậy các call tuần tự cộng dồn latency mà không bị giới hạn bởi contract tổng.

## 4. Why the fix works
Tạo linked `CancellationTokenSource` từ `requestAborted`, gọi `CancelAfter(BusinessBudget)`, và truyền token của scope đó cho mọi downstream call. Tất cả work chia sẻ cùng một deadline thay vì mỗi call có một lifetime độc lập.

Ví dụ phần cần thay trong `GetSummaryAsync`:

```csharp
using var budget = CancellationTokenSource.CreateLinkedTokenSource(requestAborted);
budget.CancelAfter(BusinessBudget);
var inventory = await dependency.CallAsync("inventory", budget.Token);
var carrier = await dependency.CallAsync("carrier", budget.Token);
return $"{inventory}+{carrier};budget={BusinessBudget.TotalMilliseconds:0}";
```

## 5. How to verify
`./verify.ps1` kiểm tra slow path bị cancel gần deadline và fast path vẫn trả đủ aggregate result.

## 6. Alternative fixes
Trong ASP.NET Core production code, deadline có thể được tạo ở middleware/application boundary rồi propagate xuống service graph. Khi downstream protocol hỗ trợ deadline riêng, có thể chuyển remaining budget thành timeout phù hợp cho dependency đó.

## 7. Wrong / misleading fixes
- Tăng `HttpClient.Timeout`: làm budget rộng hơn chứ không enforce SLO tổng.
- Đặt 900ms timeout riêng cho từng call: hai call tuần tự vẫn có thể tiêu thụ gần 1800ms.
- `Task.Run` quanh HTTP work: không tạo deadline và không giải quyết lifetime.
- Chỉ đo elapsed rồi log warning sau khi hoàn thành: quan sát được vi phạm nhưng không ngăn wasted work.

## 8. Production implications
Deadline cần propagate qua các layer; nếu layer dưới bỏ qua cancellation, caller hết budget nhưng downstream work vẫn tiếp tục. Metrics nên phân biệt caller cancellation, deadline exhaustion và dependency timeout.

## 9. Trade-offs
Budget quá ngắn có thể tăng partial failure; budget quá dài làm tail latency và resource occupancy tăng. Khi có nhiều dependencies, cần quyết định sequential/parallel execution và phân bổ remaining budget theo business priority.

## 10. Senior engineer should notice
`HttpClient.Timeout` là per-client/per-request mechanism, còn SLO thường thuộc end-to-end operation. Senior engineer phải model lifetime từ business boundary và propagate nó xuyên qua dependency graph, thay vì cấu hình timeout rời rạc.