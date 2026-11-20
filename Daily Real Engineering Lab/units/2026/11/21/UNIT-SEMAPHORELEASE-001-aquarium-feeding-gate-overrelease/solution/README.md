# Reference solution — spoiler

## Root cause
`WaitAsync(token)` ném `OperationCanceledException` trước khi chiếm permit. Khối `finally` ngoài cùng vẫn gọi `Release()`, làm tăng số permit đang khả dụng dù request chưa từng sở hữu permit. Với `SemaphoreSlim(1,1)`, `Release` có thể ném `SemaphoreFullException` ngay khi permit đang rảnh; ở fixture này permit đang được request đầu giữ nên release sai được chấp nhận, cho phép request thứ hai đi vào đồng thời.

## Fix
Chỉ vào `try/finally` quản lý permit sau khi `WaitAsync` hoàn thành. Không swallow cancellation và không release permit khi chưa acquire.

Thay toàn bộ `ExecuteAsync` bằng:
```csharp
private async Task ExecuteAsync(CancellationToken cancellationToken, Func<Task> action)
{
    await _gate.WaitAsync(cancellationToken);
    try
    {
        var active = Interlocked.Increment(ref _inside);
        UpdatePeak(active);
        try { await action(); }
        finally { Interlocked.Decrement(ref _inside); }
    }
    finally { _gate.Release(); }
}
```

## Verification
Chạy `./scripts/verify.ps1` trên **starter đã sửa**. Không chạy solution thay cho workspace. Expect `maxConcurrent=1`, exit code 0.

## Wrong fixes
- `catch (OperationCanceledException) { }` rồi tiếp tục action: thực thi khi chưa sở hữu permit.
- Tăng `SemaphoreSlim` initialCount: che triệu chứng, phá contract độc quyền.
- Bỏ `finally`: exception trong action làm kẹt permit.
- Thay bằng `lock` quanh `await`: không hỗ trợ await trong monitor lock.

## Trade-offs
SemaphoreSlim giới hạn concurrency trong một process; nếu nhiều worker/replica cùng điều khiển một thiết bị, cần coordination phân tán hoặc fencing tại thiết bị. Cancellation không đảo ngược lệnh đã gửi tới thiết bị. Đo peak, timeout, queue depth và cancellation count để kiểm tra vận hành.
