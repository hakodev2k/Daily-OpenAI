# Reference Solution — chỉ xem sau khi reproduce và tự thử fix

## 1. Symptoms
Middleware audit log được JSON đầy đủ nhưng endpoint nhận body ở trạng thái không còn dữ liệu và trả `400`.

## 2. Evidence
Cùng request hoạt động khi bỏ middleware đọc body. Audit reader chạy trước endpoint và đọc stream đến cuối.

## 3. Root cause
Request body là stream. Middleware đã consume body trước downstream. Đọc để audit nhưng không chuẩn bị body cho nhiều lần đọc và không đưa vị trí đọc về đầu khiến endpoint quan sát phần còn lại của stream thay vì original payload.

## 4. Why the fix works
Trước khi audit đọc, gọi `context.Request.EnableBuffering()`. Sau khi đọc xong, đặt `context.Request.Body.Position = 0` rồi mới `await next(context)`. Buffering cung cấp seekable/re-readable request body cho scenario này; rewind khôi phục vị trí downstream cần đọc.

Ví dụ phần middleware:

```csharp
app.Use(async (context, next) =>
{
    if (context.Request.ContentLength is > 0)
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var payload = await reader.ReadToEndAsync();
        Console.WriteLine($"AUDIT payload={payload}");
        context.Request.Body.Position = 0;
    }

    await next(context);
});
```

## 5. How to verify
Build và chạy chính `starter/` đã sửa. Cùng JSON phải trả `201`, response giữ đúng `orderId`/`sku`, và audit log vẫn có payload. Malformed JSON vẫn phải bị reject.

## 6. Alternative fixes
Nếu audit chỉ cần structured fields, có thể log sau model binding ở boundary phù hợp thay vì luôn capture raw body. Với payload lớn/sensitive, cân nhắc selective logging hoặc metadata thay vì toàn bộ body.

## 7. Wrong / Tempting Fixes
- Bỏ audit middleware: hết symptom nhưng mất requirement.
- Đọc body lần nữa ở endpoint bằng một API khác: không giải quyết ownership/stream position.
- Copy mọi request body không giới hạn vào memory: có thể đúng chức năng nhưng tạo memory/DoS risk với payload lớn.
- Nuốt `JsonException` và trả success: che lỗi và phá contract.

## 8. Production implications
Raw-body logging có privacy, secret/PII, memory, disk và observability-cost implications. Cần request-size limits, redaction và policy rõ ràng.

## 9. Trade-offs
`EnableBuffering` đơn giản và phù hợp khi thật sự cần đọc body nhiều lần, nhưng buffering có chi phí. Với traffic/payload lớn, nên giảm lượng dữ liệu capture hoặc đổi audit boundary.

## 10. What a Senior engineer should notice
Vấn đề không chỉ là một API call; đây là ownership/lifetime contract của request stream trong pipeline. Middleware phải để downstream nhìn thấy request theo contract mà nó mong đợi, đồng thời audit design phải xét data sensitivity và resource bounds.