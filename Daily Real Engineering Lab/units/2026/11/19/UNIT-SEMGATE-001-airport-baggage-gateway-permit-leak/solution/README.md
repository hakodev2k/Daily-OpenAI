# Reference Solution — chỉ xem sau khi tự reproduce và thử sửa

## Symptoms
Sau lỗi đối tác, request hợp lệ cuối cùng timeout; số permit khả dụng giảm từ 2 xuống 0.

## Evidence
Replay tuần tự có kết quả trước sửa `OK,PARTNER_ERROR,OK,PARTNER_ERROR` nhưng `availablePermits=0`. Thử thêm request hợp lệ sẽ timeout. Lỗi không do CPU hay số thread tăng.

## Root cause
`WaitAsync` lấy permit thành công, nhưng đường exception thoát trước khi gọi `Release`. Mỗi lỗi giữ lại một permit vĩnh viễn trong vòng đời gateway.

## Why the fix works
Đặt phần xử lý đối tác trong `try` và `Release()` trong `finally`, chỉ sau khi `WaitAsync` hoàn tất thành công. Điều này bảo đảm release đúng một lần trên cả success, exception và cancellation trong vùng xử lý.

## How to verify
So sánh `solution/Program.cs` với starter, tự sửa `starter/Program.cs`, rồi chạy `./verify.ps1`. Kỳ vọng `PASS`, mã thoát 0 và `availablePermits=2`. Chạy lại nhiều lần. Không thay `verify.ps1`.

## Alternative fixes
Có thể tạo abstraction async lease (`IAsyncDisposable`) để quản lý permit nhất quán tại nhiều call sites; chi phí abstraction chỉ đáng khi có nhiều vùng cần quản lý.

## Wrong / tempting fixes
Tăng max concurrency chỉ trì hoãn cạn permit. Thêm retry bên ngoài có thể làm cạn nhanh hơn. Gọi `Release` trong `catch` riêng lẻ dễ bỏ sót cancellation và success; release trong `finally` bao quanh cả `WaitAsync` có nguy cơ over-release nếu acquire thất bại.

## Production implications
Theo dõi inflight requests, queue wait, timeout phân loại theo nguyên nhân, số permit và lỗi downstream; thiết lập circuit breaker phù hợp nhưng không thay thế quản lý tài nguyên đúng.

## Trade-offs
Semaphore giới hạn tải đối tác nhưng có thể tăng queue latency; cần budget timeout cho cả chờ permit và gọi đối tác. Fail-fast khi quá tải là quyết định SLA, không phải fix của permit leak.

## Senior engineer should notice
Một incident có thể tiếp tục sau khi dependency phục hồi vì trạng thái tài nguyên nội bộ bị rò. Phải chứng minh bằng invariant acquire/release, không chỉ dựa trên correlation lỗi 503.

## References
- https://learn.microsoft.com/dotnet/api/system.threading.semaphoreslim.waitasync
- https://learn.microsoft.com/dotnet/api/system.threading.semaphoreslim.release
- https://learn.microsoft.com/dotnet/csharp/language-reference/statements/exception-handling-statements
