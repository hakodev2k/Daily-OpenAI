# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Sau ba downstream failures, `SemaphoreSlim.CurrentCount` về 0. Healthy work tiếp theo timeout khi chờ capacity mặc dù không còn operation nào thực sự chạy.

## Evidence
Mỗi failing operation gọi `WaitAsync`, giảm permit count, rồi throw trước khi code đi đến `Release()`.

## Root cause
Concurrency permit là một resource/accounting token nhưng release chỉ nằm trên success path. Exception path làm mất permit vĩnh viễn, khiến effective capacity giảm dần cho đến khi worker không còn tiến triển.

## Why the fix works
Ghép acquire/release theo cùng lifecycle và đặt release trong `finally` sau khi acquire thành công:

```csharp
await gate.WaitAsync(cancellationToken);
try
{
    Console.WriteLine($"START item={item} permits={gate.CurrentCount}");
    await Task.Delay(50, cancellationToken);

    if (downstreamFails)
    {
        throw new InvalidOperationException($"Downstream failed for {item}");
    }

    Console.WriteLine($"DONE item={item} permits={gate.CurrentCount}");
}
finally
{
    gate.Release();
}
```

## How to verify
Chạy `verify.ps1`. Healthy operation phải hoàn tất và `FINAL permits=3`.

## Alternative fixes
- Dùng abstraction quản lý lease/disposable để encode acquire/release thành một scope.
- Dùng `Channel<T>` hoặc worker-pool cố định nếu bài toán thực chất là bounded work queue.
- Dùng `Parallel.ForEachAsync` với `MaxDegreeOfParallelism` khi phù hợp với workload.

## Wrong or misleading fixes
- Tăng initial permit count: chỉ trì hoãn failure.
- Retry healthy item nhiều lần: capacity đã bị mất nên retry không sửa invariant.
- Bắt exception và bỏ qua: permit vẫn không được trả.
- Recreate semaphore định kỳ: che root cause và có thể phá concurrency guarantees.

## Production implications
Permit leak thường biểu hiện như gradual degradation: CPU thấp, process sống, queue tăng và latency tăng. Điều này dễ bị nhầm với downstream chậm hoặc thiếu worker.

## Trade-offs
`SemaphoreSlim` đơn giản và hiệu quả nhưng yêu cầu lifecycle discipline. Abstraction dạng lease giúp an toàn hơn nhưng thêm code. Worker-pool/channel có thể rõ ownership hơn cho pipeline dài hạn.

## What a Senior engineer should notice
Mọi concurrency limiter đều có accounting invariant. Acquire/release phải cân bằng trên success, exception và cancellation paths. Khi throughput giảm dần sau lỗi nhưng CPU thấp, hãy kiểm tra resource/permit/connection leaks trước khi scale out.
