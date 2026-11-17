# Reference solution — spoiler

## Root cause
`WaitAsync(token)` có thể ném `OperationCanceledException` trước khi cấp permit. `finally` bên ngoài vẫn gọi `Release()` dù chưa sở hữu permit. Khi A vẫn đang xử lý, B hủy sẽ làm permit xuất hiện, khiến C được vào sớm. `SemaphoreSlim(1,1)` có thể ném `SemaphoreFullException` khi A kết thúc, nhưng lỗi xảy ra sau khi vi phạm giới hạn đã diễn ra.

## Fix
Chỉ đặt `try/finally` giải phóng permit **sau khi** `WaitAsync` hoàn tất thành công. Xem `Program.cs` ở thư mục này.

## Verify
```powershell
./verify.ps1
```
So sánh `VIOLATION` và `PASS`. Cần kiểm tra cả trường hợp work ném exception: permit vẫn phải được trả.

## Wrong fixes
- Bỏ `finally`: permit bị leak khi work thất bại.
- Bắt rồi nuốt `OperationCanceledException`: che giấu cancellation nhưng không bảo đảm ownership.
- Tăng giới hạn semaphore: che triệu chứng và phá invariant.

## Trade-off
Semaphore chỉ giới hạn đồng thời trong một process; nhiều instance cần cơ chế điều phối phân tán hoặc partition theo tài nguyên. Với workflow production, cần metrics về queue wait, cancellations, active permits và latency.
