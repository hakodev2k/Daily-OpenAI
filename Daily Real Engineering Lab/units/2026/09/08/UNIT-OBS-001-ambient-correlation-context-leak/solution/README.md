# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Operation B không đặt correlation mới nhưng vẫn log `corr-A`.

## Evidence
`AsyncLocal<string?>` giữ giá trị trong execution context hiện tại. Starter chỉ set khi có giá trị mới và không restore/clear khi operation A kết thúc.

## Root cause
Ambient correlation state có lifetime dài hơn logical operation sở hữu nó. Đây là scope leak trong observability context.

## Why the fix works
Capture giá trị cũ, set context cho operation hiện tại, rồi restore trong `finally`:

```csharp
static async Task HandleAsync(string name, string? correlationId)
{
    var previous = CorrelationContext.Current.Value;

    try
    {
        CorrelationContext.Current.Value = correlationId;
        await Task.Delay(10);
        Console.WriteLine($"{name}: correlation={CorrelationContext.Current.Value ?? "<null>"}");
    }
    finally
    {
        CorrelationContext.Current.Value = previous;
    }
}
```

## How to verify
Chạy `verify.ps1`. Operation A phải log `corr-A`, operation B phải log `<null>`.

## Alternative fixes
- Encapsulate correlation state trong một disposable scope.
- Dùng logging scope của framework thay vì tự quản lý ambient state.
- Truyền correlation ID explicit nếu code path ngắn và clarity quan trọng hơn convenience.

## Wrong or misleading fixes
- Set correlation của B thành string rỗng: chỉ che stale state cho một call-site.
- Clear context chỉ trên success path: exception vẫn gây leak.
- Dùng static mutable string thay cho `AsyncLocal`: làm isolation tệ hơn khi có concurrency.

## Production implications
Telemetry corruption làm root-cause analysis sai, trace stitching sai và có thể khiến incident investigation đi nhầm hướng dù business data vẫn đúng.

## Trade-offs
Ambient context giảm parameter plumbing nhưng tăng hidden coupling và yêu cầu lifecycle discipline. Explicit context verbose hơn nhưng dễ reasoning hơn.

## What a Senior engineer should notice
Observability context cũng là state. Mọi ambient state cần owner, scope và cleanup semantics rõ ràng như transaction, lock, permit hoặc request scope.
