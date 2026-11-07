# Reference Solution

> Spoiler: chỉ đọc sau khi đã reproduce và tự thử sửa.

## Symptoms

Response gzip có thể rất nhỏ trên wire nhưng logical body sau decompression lớn hơn nhiều. Starter guard không enforce được policy trên body mà application thực sự sử dụng.

## Root cause

`SocketsHttpHandler.AutomaticDecompression` xử lý content encoding trước khi application đọc content. Guard starter dựa vào response metadata rồi gọi `ReadAsByteArrayAsync()`, vì vậy không có running bound trên số bytes post-decompression được materialize.

Đây là boundary mismatch: policy được mô tả như memory/body safety limit nhưng evidence/check lại không đo chính boundary cần bảo vệ.

## Reference approach

`solution/Program.cs` dùng `ResponseHeadersRead`, đọc content stream theo chunk, đếm số bytes application nhận và abort ngay khi vượt budget.

Điểm quan trọng không phải tên API cụ thể mà là:

1. không full-buffer trước khi quyết định
2. đo đúng post-processing boundary
3. enforce budget trong lúc streaming
4. fail closed khi vượt budget

## Wrong fixes

### Chỉ tăng limit

Chỉ dời threshold và không giải quyết mismatch giữa wire size với consumed size.

### Kiểm tra Content-Length rồi vẫn ReadAsByteArrayAsync

Metadata không phải một running bound trên post-decompression body.

### Disable gzip

Có thể làm bandwidth tăng mạnh và thay đổi integration contract; đây không phải default fix cho resource safety.

### Download toàn bộ rồi kiểm tra Length

Policy được enforce quá muộn: memory cost đã xảy ra.

## Production reasoning

Trong production, cân nhắc đồng thời:

- decompressed byte budget
- request deadline/cancellation
- destination streaming thay vì MemoryStream nếu downstream cho phép
- concurrency limits
- telemetry cho rejected payloads
- partner contract về maximum logical object size

Nếu dữ liệu được deserialize, còn phải xét thêm object-graph expansion và parser-specific limits.

## Trade-offs

Streaming thêm code và yêu cầu lifecycle ownership rõ hơn, nhưng cho phép backpressure/bounded memory. Nếu business thực sự cần payload lớn, thiết kế nên stream đến bounded destination thay vì chỉ tăng in-memory threshold.
