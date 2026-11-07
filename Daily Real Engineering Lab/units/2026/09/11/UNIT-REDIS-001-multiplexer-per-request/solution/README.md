# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Functional result đúng nhưng burst traffic tạo gần một client/handshake cho mỗi request, làm latency bị khuếch đại dù cache lookup rất nhẹ.

## Evidence
`clientsCreated` và `handshakes` tăng gần bằng request count. Điều này cho thấy connection setup đang nằm trên request path.

## Root cause
Connection-oriented Redis client được tạo bên trong mỗi request thay vì được reuse. Với StackExchange.Redis, `ConnectionMultiplexer` được thiết kế để được share/reuse; tạo mới liên tục làm mất pooling/multiplexing benefit và tăng connection churn.

## Why the fix works
Khởi tạo client một lần ở composition root rồi inject/reuse nó cho các request. Nhiều logical operations dùng chung connection-oriented client thay vì lặp handshake.

## How to verify
Chạy `./verify.ps1`. Kết quả phải vẫn đúng và `clientsCreated`/`handshakes` không vượt quá 2.

## Alternative fixes
- Singleton registration qua DI.
- Lazy singleton nếu startup connection cần defer.
- Managed connection factory với lifecycle rõ ràng nếu có nhiều Redis endpoints.

## Wrong / tempting fixes
- Tăng timeout: che symptom nhưng vẫn churn connection.
- Scale out API ngay: có thể làm tổng connection churn tệ hơn.
- Tạo connection pool thủ công quanh một client vốn đã hỗ trợ multiplexing: tăng complexity không cần thiết.
- Cache kết quả trong memory để tránh Redis hoàn toàn: có thể hợp lý trong context khác, nhưng không sửa lifecycle defect đang được đo.

## Production implications
Theo dõi connection count, reconnect rate, timeout rate và latency. Lifecycle của client phải khớp với semantics của thư viện thực tế; không áp dụng singleton máy móc cho mọi type.

## Trade-offs
Shared client giảm setup cost và tận dụng multiplexing, nhưng trở thành shared dependency dài hạn: cần health/telemetry, reconnect behavior và configuration phù hợp.

## Senior engineer should notice
Đừng tối ưu cache lookup trước khi đo resource lifecycle. Khi functional correctness xanh nhưng latency xấu dưới concurrency, hãy tìm work bị nhân theo request: connection setup, serialization, retries, locks và downstream fan-out.