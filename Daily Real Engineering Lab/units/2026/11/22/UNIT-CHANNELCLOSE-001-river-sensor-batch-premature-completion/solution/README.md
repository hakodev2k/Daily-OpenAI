# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
`received < expected`; các producer còn lại gặp `ChannelClosedException`, nhưng exception bị gom vào `producerErrors` nên không nổi lên top-level.

## Evidence
Ba producer có nhịp gửi khác nhau. `Task.WhenAny(producers)` chỉ chờ một producer, trong khi channel vẫn còn các writer hoạt động. Khi channel bị complete, những writer đó không thể gửi phần còn lại.

## Root cause
Tín hiệu hoàn tất của writer được phát trước khi mọi producer hoàn thành. `ReadAllAsync` kết thúc đúng theo channel lifecycle, nhưng lifecycle bị điều phối sai.

## Corrected coordination
Thay đoạn từ `await Task.WhenAny(producers);` đến `await consumer;` bằng:

```csharp
await Task.WhenAll(producers);
channel.Writer.TryComplete();
await consumer;
```

Trong production, nếu producer exception cần propagate, không swallow; bảo đảm complete channel trong `finally` hoặc truyền exception tới `TryComplete(error)` và chờ consumer để tránh treo.

## Why it works
Tất cả producer đã gửi xong trước khi writer phát tín hiệu complete. Consumer tiếp tục drain mọi bản ghi còn trong buffer rồi mới kết thúc.

## How to verify
Chạy `pwsh ./verify.ps1`: exit code 0, `expected=12 received=12 unique=12 missing=0 producerErrors=0`. Chạy lặp lại 20 lần và đổi delay để kiểm tra interleaving.

## Alternatives
Dùng `Task.WhenAll` với một task chuyên quản lý writer và `try/finally`; hoặc mỗi producer gửi vào một pipeline độc lập và fan-in có coordinator.

## Wrong fixes
Tăng `Task.Delay` trước `TryComplete` chỉ che race; dùng `Wait` đồng bộ làm mất async scalability; bỏ `TryComplete` có thể khiến consumer chờ mãi.

## Production implications
Giám sát expected/received, failed producer, channel completion cause, drain latency và batch correlation ID. Xử lý cancellation và backpressure với bounded channel khi cần.

## Trade-offs
Unbounded channel đơn giản nhưng có nguy cơ tăng memory; bounded channel bảo vệ memory nhưng yêu cầu thiết kế backpressure. Fail-fast và partial batch acceptance là quyết định nghiệp vụ.

## Senior engineering insight
`Task.WhenAll` đảm bảo tất cả task hoàn thành, không đảm bảo thành công nếu lỗi bị swallow. Cần thiết kế ownership, error propagation, completion order và observability như một contract.
