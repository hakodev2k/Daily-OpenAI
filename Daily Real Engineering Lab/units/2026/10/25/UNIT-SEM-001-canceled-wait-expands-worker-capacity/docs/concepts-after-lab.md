# Concepts after lab — read after your own attempt
`SemaphoreSlim.WaitAsync` là thao tác acquire bất đồng bộ. Token bị hủy trước khi semaphore cấp permit khiến task bị canceled, nên operation không sở hữu permit nào. `finally` chỉ an toàn nếu đặt trong phạm vi đã acquire thành công.

`SemaphoreSlim(initialCount)` không mặc định có max count bằng initial count. Vì vậy over-release có thể không ném exception ngay mà làm số permit khả dụng lớn hơn cấu hình. `SemaphoreSlim(initialCount, maxCount)` hữu ích để phát hiện over-release, nhưng ownership đúng vẫn là yêu cầu chính.

Trong test, `TaskCompletionSource(RunContinuationsAsynchronously)` giữ A ở trạng thái đang chạy cho tới khi test chủ động thả; C được kiểm tra trước khi thả A. Điều này kiểm tra thứ tự nhân quả, không dựa vào scheduler delay hay stopwatch.

Production: phân biệt cancellation trong queue, cancellation khi đang render và exception từ renderer. Nếu có nhiều node, semaphore local không bảo đảm giới hạn toàn hệ thống.
