# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Audit log chứa đúng webhook payload nhưng endpoint phía sau trả `400 Bad Request`.

## Evidence
Middleware đọc `Request.Body` đến cuối. Khi endpoint chạy, stream position đã ở cuối nên deserializer không còn byte nào để đọc.

## Root cause
Request body stream bị consume bởi middleware audit mà không được cấu hình buffering và rewind trước khi chuyển tiếp request.

## Why the fix works
Enable buffering trước khi đọc, sau đó đặt lại position về 0 để downstream có thể đọc cùng payload:

```csharp
context.Request.EnableBuffering();

using var reader = new StreamReader(
    context.Request.Body,
    Encoding.UTF8,
    detectEncodingFromByteOrderMarks: false,
    leaveOpen: true);

var rawBody = await reader.ReadToEndAsync();
Console.WriteLine($"AUDIT_BODY={rawBody}");

context.Request.Body.Position = 0;
await next();
```

## How to verify
Chạy `verify.ps1`. Endpoint phải trả 200 và echo đúng payload.

## Alternative fixes
- Audit metadata thay vì full body nếu không cần raw payload.
- Capture body tại một boundary chuyên trách rồi truyền typed payload xuống pipeline.
- Với payload lớn, áp dụng size limit và buffering threshold phù hợp.

## Wrong or misleading fixes
- Catch JSON exception và trả 200: che symptom, mất dữ liệu.
- Move audit middleware sau endpoint: không giải quyết nhu cầu audit raw body trước xử lý.
- Buffer mọi request không giới hạn: tăng memory/disk pressure.

## Production implications
Logging raw request body có thể chứa PII hoặc secrets. Cần redaction, retention policy, access control và giới hạn kích thước.

## Trade-offs
Buffering giúp nhiều component đọc body nhưng tăng resource usage. Chỉ bật cho route cần thiết.

## What a Senior engineer should notice
Observability middleware không được làm thay đổi request semantics. Các primitive dạng stream cần được xử lý như shared mutable state có lifecycle rõ ràng.
