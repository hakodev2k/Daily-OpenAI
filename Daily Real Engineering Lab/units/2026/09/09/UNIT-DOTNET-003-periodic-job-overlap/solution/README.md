# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms
Khi thời gian xử lý vượt chu kỳ timer, nhiều execution cùng xuất hiện trong log và `maxActive` lớn hơn 1.

## Evidence
Timestamp cho thấy execution mới bắt đầu trước khi execution trước kết thúc. `active=` tăng lên 2 hoặc cao hơn trong starter.

## Root cause
`System.Threading.Timer` kích hoạt callback theo period đã cấu hình và không tự chờ một callback async trước đó hoàn tất. Callback `async void` sinh ra bởi lambda của `TimerCallback` khiến work có thể re-enter khi tick kế tiếp đến.

## Why the fix works
Chuyển scheduling sang một async loop tuần tự với `PeriodicTimer`: mỗi vòng `await WaitForNextTickAsync()`, sau đó `await` toàn bộ work trước khi quay lại chờ tick tiếp theo. Completion của work trở thành một phần của lifecycle scheduling.

## How to verify
Chạy `verify.ps1`. Kết quả phải có `maxActive=1` và `executions` lớn hơn hoặc bằng 2.

## Alternative fixes
- Dùng một non-blocking guard như `SemaphoreSlim` để bỏ qua hoặc serialize tick, nếu semantics bỏ tick được định nghĩa rõ.
- Reschedule one-shot timer chỉ sau khi work hoàn tất.
- Dùng scheduler/framework có explicit non-overlap policy nếu hệ thống đã phụ thuộc vào scheduler đó.

## Wrong or misleading fixes
- Chỉ tăng period: giảm xác suất overlap nhưng không tạo invariant nếu runtime của work vẫn có thể dài hơn period.
- Dùng `lock` bao quanh code async: không phù hợp với `await` và dễ dẫn tới thiết kế blocking.
- Chỉ kiểm tra một boolean không đồng bộ: check/set không atomic nên vẫn có race.
- Tăng ThreadPool: không sửa scheduling semantics.

## Production implications
Overlap có thể nhân đôi tải downstream, tạo contention và làm một job vốn ổn định trở nên khó dự đoán khi latency tăng. Cần quyết định rõ semantics: serialize, skip tick, queue tick hay cho phép overlap.

## Trade-offs
Async loop tuần tự đơn giản và dễ reasoning nhưng cadence trở thành "delay/tick + work duration" khi work dài. Nếu business yêu cầu fixed-rate scheduling, cần policy rõ cho missed ticks và backpressure thay vì mặc định serialize.

## What a Senior engineer should notice
Periodic scheduling là một concurrency contract. Trước khi chọn API, cần xác định invariant về overlap, missed execution, shutdown, cancellation, retry và idempotency thay vì chỉ cấu hình một khoảng thời gian.
