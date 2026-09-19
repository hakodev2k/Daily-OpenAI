# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Hai producer đã hoàn tất và consumer đã xử lý đủ 6 event, nhưng consumer task không kết thúc nên shutdown chờ tới timeout.

## 2. Evidence
`producers=completed`, `processed-count=6` và `consumer-completed=False` xuất hiện cùng lúc. Không có event bị kẹt trong buffer; consumer đang chờ khả năng có dữ liệu mới.

## 3. Root cause
Pipeline không có owner chịu trách nhiệm kết thúc lifecycle của `ChannelWriter`. `ReadAllAsync()` không suy luận rằng “mọi producer task hiện tại đã xong” đồng nghĩa với “sẽ không bao giờ có write mới”. Vì writer chưa được completed, reader hợp lệ tiếp tục chờ.

## 4. Why the fix works
Sau khi boundary quản lý producer xác nhận toàn bộ producer đã kết thúc, nó phải hoàn tất writer rồi chờ consumer drain phần dữ liệu còn lại:

```csharp
await producers;
channel.Writer.Complete();
await consumer;
```

Trong hệ thống thực tế nên đặt quyền completion ở coordinator biết đầy đủ lifecycle producer; producer riêng lẻ không nên tự complete channel khi vẫn còn producer khác.

## 5. How to verify
Sửa `starter/Program.cs`, sau đó chạy `./verify.ps1`. Kết quả phải có `processed-count=6`, `consumer-completed=True` và `VERIFY_PASS`.

## 6. Alternative fixes
Có thể encapsulate channel trong component sở hữu producer registration và tự complete khi producer cuối cùng rời đi. Với pipeline gắn chặt vào host cancellation, có thể dùng cancellation để buộc dừng, nhưng cancellation và normal completion biểu diễn hai semantics khác nhau.

## 7. Wrong or misleading fixes
Tăng shutdown timeout chỉ kéo dài thời gian chờ. Cancel consumer ngay sau producers có thể bỏ dữ liệu đang buffer. Cho producer đầu tiên gọi `Complete()` tạo race khiến producer còn lại không write được.

## 8. Production implications
Lifecycle không rõ ràng có thể làm deployment chậm, trigger forced termination, mất telemetry hoặc khiến health state không phản ánh đúng việc drain đã hoàn tất.

## 9. Trade-offs
Explicit completion yêu cầu xác định ownership rõ ràng. Dynamic producer topology cần coordinator/ref-count hoặc abstraction khác; không nên hard-code completion ở một producer bất kỳ.

## 10. What a Senior engineer should notice
Channel không chỉ là queue dữ liệu mà còn có lifecycle protocol. Cần thiết kế ownership cho producer registration, normal completion, fault propagation và cancellation riêng biệt thay vì dùng timeout làm cơ chế điều phối.
