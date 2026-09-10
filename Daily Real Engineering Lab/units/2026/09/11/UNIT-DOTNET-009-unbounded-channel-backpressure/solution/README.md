# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Producer hoàn thành gần như ngay lập tức, trong khi consumer xử lý tuần tự với độ trễ cố ý. `maxBacklog` tăng mạnh dù cuối cùng `produced` và `consumed` vẫn bằng nhau.

## Evidence

Starter in ra bốn giá trị: `produced`, `consumed`, `maxBacklog`, `producerElapsedMs`. Evidence quan trọng là backlog lớn trong khi correctness cuối cùng vẫn đúng. Đây là dấu hiệu capacity control thiếu, không phải lỗi business logic ở consumer.

## Root cause

Starter dùng `Channel.CreateUnbounded<int>`. Với unbounded channel, `WriteAsync` không tạo backpressure trong tình huống này; producer có thể tiếp tục enqueue nhanh hơn consumer drain queue. Khi workload thực tế lớn hơn và item giữ object graph/payload đáng kể, backlog biến thành memory pressure và latency nội bộ.

## Why the fix works

Reference solution dùng `Channel.CreateBounded<int>` với capacity 8 và `BoundedChannelFullMode.Wait`. Khi buffer đầy, `WriteAsync` chờ consumer giải phóng capacity. Producer vì vậy tự điều chỉnh theo khả năng downstream của pipeline thay vì tiếp tục tích lũy work vô hạn.

## How to verify

Sửa trực tiếp `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Verification yêu cầu:

- `produced=200`
- `consumed=200`
- `maxBacklog <= 8`

## Alternative fixes

- Giới hạn admission ở lớp trước channel nếu business contract cho phép reject/throttle upstream.
- Scale consumer khi evidence cho thấy consumer thực sự là bottleneck và scaling phù hợp với dependency limits.
- Batch work để tăng throughput nếu workload cho phép mà không phá latency/SLA.
- Dùng drop policy (`DropOldest`, `DropNewest`, `DropWrite`) chỉ khi mất work là một quyết định product rõ ràng, ví dụ telemetry best-effort.

## Wrong or misleading fixes

- Tăng memory/heap limit: chỉ kéo dài thời gian trước khi backlog gây sự cố.
- Tăng ThreadPool minimum threads: không giải quyết việc producer không bị giới hạn.
- Thêm nhiều consumer vô điều kiện: có thể đẩy bottleneck sang database/downstream API.
- Dùng bounded channel nhưng chọn drop mode trong workflow không được phép mất dữ liệu: giảm backlog bằng cách phá correctness.
- Gọi `TryWrite` rồi bỏ qua `false`: biến capacity control thành silent data loss.

## Production implications

Capacity là một phần của system contract. Giá trị quá nhỏ có thể làm throughput thấp hoặc tăng producer latency; quá lớn có thể che giấu bottleneck và giữ quá nhiều dữ liệu trong memory. Cần chọn capacity từ workload size, consumer throughput, acceptable buffering time và memory budget.

## Trade-offs

Backpressure cố ý làm producer chậm lại. Đây không phải regression nếu hệ thống trước đó chỉ đạt throughput đầu vào bằng cách tích lũy work không giới hạn. Tuy nhiên, khi producer là request path, cần quyết định rõ client sẽ chờ, bị throttle hay nhận asynchronous acceptance.

## What a Senior engineer should notice

- Queue depth/backlog là tín hiệu capacity, không chỉ là metric vận hành phụ.
- Correctness cuối cùng (`produced == consumed`) không chứng minh pipeline khỏe.
- Backpressure phải tồn tại ở boundary phù hợp; nếu chỉ chuyển queue không giới hạn sang thành phần khác thì vấn đề chưa được giải quyết.
- Bounded buffers cần đi cùng cancellation, shutdown behavior và failure handling trong production.
