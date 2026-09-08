# Reference Solution

## Root cause
Starter chỉ đặt timeout cho từng attempt. Mỗi lần retry tạo một budget mới nên ba attempt 300 ms có thể khiến tổng operation kéo dài gần 900 ms, vượt latency budget 500 ms.

## Fix
Tạo một cancellation source đại diện cho deadline tổng thể và một cancellation source cho từng attempt. Liên kết cả hai token khi gọi dependency. Khi overall deadline hết, dừng retry và trả kết quả timeout tổng thể.

Ví dụ:

```csharp
using var overall = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));

for (var attempt = 1; attempt <= maxAttempts; attempt++)
{
    if (overall.IsCancellationRequested)
        break;

    using var perAttempt = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
    using var linked = CancellationTokenSource.CreateLinkedTokenSource(overall.Token, perAttempt.Token);

    try
    {
        await CallDependencyAsync(linked.Token);
        Console.WriteLine("RESULT=success");
        return;
    }
    catch (OperationCanceledException) when (overall.IsCancellationRequested)
    {
        Console.WriteLine("RESULT=overall-timeout");
        return;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine($"ATTEMPT_{attempt}=timeout");
    }
}

Console.WriteLine("RESULT=overall-timeout");
```

## Why it works
`perAttempt` giới hạn một lần gọi riêng lẻ; `overall` bảo vệ latency budget của toàn operation. Linked token đảm bảo dependency dừng khi một trong hai budget hết hạn.

## Wrong fixes
- Chỉ giảm per-attempt timeout mà không có deadline tổng thể.
- Tăng số retry khi downstream đang chậm.
- Catch `OperationCanceledException` rồi retry vô hạn.
- Đặt một timeout lớn hơn cho từng attempt và cho rằng đó là total budget.

## Senior takeaway
Retry tiêu thụ latency budget. Resilience policy phải reasoning theo toàn operation, không chỉ từng network call. Khi thiết kế retry cần đồng thời xem deadline, attempt count, backoff và khả năng dependency phục hồi trong phần budget còn lại.
