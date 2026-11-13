# Reference solution — Spoiler

## Root cause
Theo RFC 9110 §10.2.3, `Retry-After` nhận cả `delay-seconds` và `HTTP-date`. Starter chỉ parse số nguyên và fallback 1 giây với ngày HTTP hợp lệ. Điều này tạo retry sớm, có thể khuếch đại 429. `now` được inject để kiểm chứng xác định.

## Fix tối thiểu
Thay nhánh fallback cuối của `RetryPolicy.Delay` bằng:

```csharp
if (DateTimeOffset.TryParseExact(reply.RetryAfter, "r", CultureInfo.InvariantCulture,
    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var retryAt))
{
    var delta = retryAt - now;
    return delta <= TimeSpan.Zero ? TimeSpan.Zero
        : delta >= TimeSpan.FromSeconds(120) ? TimeSpan.FromSeconds(120) : delta;
}
return TimeSpan.FromSeconds(1);
```

## Verify
Chạy `pwsh ./scripts/verify.ps1` từ thư mục unit: 7/7 PASS. Đây là static-review; chưa chạy .NET trong phiên tạo lab.

## Wrong fixes
- Parse mọi giá trị bằng `int.Parse`: ném exception cho HTTP-date.
- `DateTime.Now` trực tiếp: test lệ thuộc timezone và clock thực.
- Sleep ngay trong parser: trộn policy với execution, khó cancel và khó test.
- Retry ngay khi 429: gây retry storm.
- Tin mọi delay vô hạn: request có thể bị treo vượt SLA.

## Trade-offs
Clamp 120 giây là policy của lab, không phải quy tắc RFC. Production cần giới hạn tổng retry budget, jitter khi thiếu header, cancellation, idempotency cho POST, và telemetry theo retry reason. HTTP-date lệ thuộc clock skew; có thể dùng server Date header làm tín hiệu phụ khi tin cậy. Không retry request không an toàn khi thiếu idempotency key.

## Reference
RFC 9110 §10.2.3: https://www.rfc-editor.org/rfc/rfc9110.html#section-10.2.3
