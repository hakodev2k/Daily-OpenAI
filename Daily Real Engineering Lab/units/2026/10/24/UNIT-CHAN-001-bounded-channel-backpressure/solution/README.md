# Reference Solution — chỉ đọc sau khi tự thử

## 1. Symptoms
Producer chấp nhận toàn bộ burst khi consumer vẫn bị giữ ở sự kiện đầu tiên. Bộ nhớ và backlog có thể tăng không giới hạn theo tốc độ nhận.

## 2. Evidence
`AllPublishedBeforeRelease=True` và `AcceptedBeforeRelease=80` chứng tỏ toàn bộ burst đã được tiếp nhận trước khi downstream giải phóng. Chỉ số `Completed` vẫn rất thấp ở thời điểm này.

## 3. Root cause
`Channel.CreateUnbounded<int>` không giới hạn số phần tử đang chờ. `WriteAsync` thường hoàn thành ngay khi có bộ nhớ, nên producer không nhận tín hiệu áp lực từ consumer. Độ lệch tốc độ sản xuất/xử lý chuyển thành backlog.

## 4. Why the fix works
Trong `DispatchQueue.cs`, dùng `Channel.CreateBounded<int>` với capacity 16, `BoundedChannelFullMode.Wait` và tiếp tục `await WriteAsync`. Khi buffer đầy, producer chờ chỗ trống. Worker đọc phần tử tiếp theo sẽ giải phóng chỗ; không drop dữ liệu. `CompleteAsync` đánh dấu kết thúc ghi và đợi consumer drain toàn bộ.

## 5. How to verify
```powershell
./verify.ps1
dotnet run --project solution/QueueLab.Solution.csproj -- verify
```
Lệnh thứ nhất kiểm tra **starter do bạn sửa**; lệnh thứ hai chỉ tự kiểm tra reference solution. Kỳ vọng `AllPublishedBeforeRelease=False`, `PeakOutstanding<=18`, `Completed=80`, không mất bản ghi.

## 6. Alternative fixes
- SemaphoreSlim trước khi tiếp nhận: hợp lệ nếu ownership và release được quản lý chặt; khó hơn khi xử lý cancellation/error.
- Queue dựa trên durable broker: hợp lý khi cần tồn tại qua process crash, nhưng tăng vận hành và không thay thế giới hạn in-flight.
- Throttle upstream: cần giao thức end-to-end rõ ràng, không thay thế local capacity contract.

## 7. Wrong / tempting fixes
- Chỉ tăng RAM hoặc số worker: hoãn triệu chứng, không bảo đảm giới hạn backlog.
- Dùng `DropOldest`/`DropWrite`: có thể hợp lệ với telemetry best-effort, nhưng vi phạm yêu cầu không mất sự kiện nhập kho.
- Dùng `TryWrite` rồi bỏ qua `false`: mất sự kiện âm thầm.
- Đổi sang `Task.Run` cho từng event: tạo thêm công việc không giới hạn thay vì áp lực ngược.

## 8. Production implications
Capacity phải được chọn theo RAM budget, kích thước message, thời gian xử lý, số consumer và SLO. Backpressure cần được truyền tới HTTP/stream ingress (ví dụ deadline, 429/503 khi quá tải theo contract), không để producer chờ vô hạn. Với nhiều replica, mỗi replica có queue riêng. Nếu mất dữ liệu khi process crash là không chấp nhận được, cần durable handoff/acknowledgment.

## 9. Trade-offs
Bounded queue đổi latency của producer để lấy memory predictability. Throughput bền vững vẫn bị giới hạn bởi downstream. Giới hạn quá nhỏ làm tăng wait; quá lớn tăng bộ nhớ và thời gian drain. Chính sách wait không đảm bảo durability.

## 10. Senior engineer should notice
`Accepted` trong harness là số lần ghi thành công vào memory queue, **không phải** cam kết durable. Test kiểm tra tính toàn vẹn trong một process và một consumer, không chứng minh exactly-once dưới crash hoặc multi-instance. Cần theo dõi queue depth, wait duration, rejected/canceled requests, drain time và xử lý failure của consumer. Counter được cập nhật sau `WriteAsync` nên chỉ là telemetry xấp xỉ khi consumer rất nhanh; kiểm tra gate ở đây loại bỏ sai lệch chính trong thời điểm quan sát.
