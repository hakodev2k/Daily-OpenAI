# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Một request tới `/tickets/fail` tạo hai audit entry có cùng logical request identifier, trong khi `/tickets/ok` chỉ tạo một.

## 2. Evidence
Audit count tăng hai lần trên failure path. Error handling đổi path sang `/error`, nhưng vẫn xử lý cùng `HttpContext` và cùng request identifier.

## 3. Root cause
`UseExceptionHandler("/error")` có thể re-execute request pipeline với path thay thế. Middleware audit nằm trong phần pipeline được đi qua lại và tạo side effect mỗi lần nó được invoke. Code đã đồng nhất “middleware invocation” với “logical request”, nhưng hai khái niệm đó không luôn tương đương trên error path.

## 4. Why the fix works
Giữ audit ở vị trí cross-cutting hiện tại nhưng làm operation idempotent trong phạm vi `HttpContext`: trước khi ghi, kiểm tra một marker trong `HttpContext.Items`; lần đầu đặt marker và ghi audit, lần re-execution chỉ tiếp tục pipeline.

Ví dụ:
```csharp
app.Use(async (context, next) =>
{
    const string AuditMarker = "lab.audit-recorded";
    if (!context.Items.ContainsKey(AuditMarker))
    {
        context.Items[AuditMarker] = true;
        var sink = context.RequestServices.GetRequiredService<AuditSink>();
        var requestId = context.Request.Headers["X-Lab-Request-Id"].FirstOrDefault() ?? context.TraceIdentifier;
        sink.Add(requestId, context.Request.Path);
    }

    await next();
});
```

## 5. How to verify
Chạy `./verify.ps1`. Script kiểm tra cả success path và failure path; cả hai phải có `audit-count=1` theo logical request identifier và kết thúc bằng `VERIFY_PASS`.

## 6. Alternative fixes
- Đặt side effect ở một pipeline boundary không bị re-execute, nếu semantics và thứ tự middleware cho phép.
- Ghi audit sau khi toàn bộ request hoàn tất từ một outer middleware, nếu audit contract chỉ cần một final outcome và code xử lý exception không làm mất control flow mong muốn.
- Chuyển audit thành event/outbox ở application boundary nếu yêu cầu durability và business semantics cao hơn request logging.

## 7. Wrong or misleading fixes
- Xóa error handler để audit count về một: mất behavior xử lý lỗi cần thiết.
- Bỏ audit trên failure path: che symptom nhưng phá audit requirement.
- Dựa vào path `/error` để skip một cách cứng nhắc: coupling với route và dễ hỏng khi error handling thay đổi.
- Dedupe toàn cục chỉ bằng URL: các request hợp lệ khác nhau có thể bị gộp nhầm.

## 8. Production implications
Re-execution ảnh hưởng mọi middleware có state hoặc side effect: audit, request-body buffering, metrics, enrichment và custom authorization logic đều cần được xem xét về reentrancy.

## 9. Trade-offs
`HttpContext.Items` phù hợp cho run-once semantics trong một request nhưng không cung cấp durability giữa process hoặc retry từ client. Nếu business side effect cần exactly-once-like behavior xuyên request, cần idempotency key và persistence boundary phù hợp.

## 10. What a Senior engineer should notice
Senior engineer cần phân biệt request-level semantics với middleware-invocation semantics, đọc error-path behavior của framework, và thiết kế cross-cutting concern sao cho reentrant hoặc đặt đúng boundary thay vì chỉ sửa symptom.
