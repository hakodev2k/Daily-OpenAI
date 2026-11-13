# Reference Solution

## Root cause
`finally` luôn gọi `gate.Release()`, kể cả khi `WaitAsync` ném `OperationCanceledException`. Với `SemaphoreSlim(1,2)`, một release sai làm tăng permit từ 0 lên 1 khi A còn giữ gate, cho phép C chạy song song với A.

## Fix
Thay phần `try/catch/finally` trong `Process` bằng logic tương đương:

```csharp
bool acquired = false;
try
{
    await gate.WaitAsync(cancellation);
    acquired = true;
    int now = Interlocked.Increment(ref active);
    int old;
    do { old = Volatile.Read(ref peak); if (now <= old) break; }
    while (Interlocked.CompareExchange(ref peak, now, old) != old);
    Console.WriteLine($"{name}: entered");
    if (name == "A") aEntered.SetResult();
    try { if (hold is not null) await hold; else await Task.Delay(15); }
    finally { Interlocked.Decrement(ref active); }
}
catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
{
    Console.WriteLine($"{name}: cancelled");
}
finally { if (acquired) gate.Release(); }
```

## Verify
Chạy `./verify.ps1` trên starter đã sửa. Kết quả phải có `MAX_ACTIVE=1`, `B: cancelled`, không có `B: entered`.

## Wrong fixes
- Tăng maxCount hoặc thêm delay: không giải quyết ownership.
- Bỏ cancellation: làm mất hành vi cần thiết.
- Nuốt exception của release: che dấu invariant bị phá.
- Chỉ lock phần cập nhật `active`: không bảo vệ công việc thực tế.

## Trade-offs
Semaphore chỉ bảo vệ trong một process. Nhiều replica cần distributed coordination và fencing token; khi lỗi công việc phải vẫn release permit đã sở hữu.
