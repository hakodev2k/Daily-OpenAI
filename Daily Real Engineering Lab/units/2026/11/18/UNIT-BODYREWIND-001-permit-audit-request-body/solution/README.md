# Reference Solution — chỉ xem sau khi tự thử
## Symptoms
Audit ghi đủ JSON; handler nhận 0 byte, trả 400.
## Evidence
Hai lần ReadToEndAsync đọc cùng một request stream. `leaveOpen` không phục hồi vị trí.
## Root cause
Audit tiêu thụ stream rồi chuyển downstream khi vị trí đọc đã ở cuối.
## Fix
Trong audit middleware, trước StreamReader thêm:
```csharp
ctx.Request.EnableBuffering();
```
Sau `auditPayload = await reader.ReadToEndAsync();`, trước `await handler(ctx);`, thêm:
```csharp
ctx.Request.Body.Position = 0;
```
## Why it works
Buffering cho phép đọc lại body khi hosting stream không seekable; reset vị trí cho downstream thấy đầy đủ payload.
## Verification
Chạy `./scripts/verify.ps1` trên **starter đã sửa**; status 202, cả audit và handler đều đọc đủ bytes, exit 0. `reproduce.ps1` phải không còn tái hiện lỗi sau sửa.
## Alternatives
Audit metadata/hash, endpoint-specific logging, hoặc tee stream có giới hạn kích thước.
## Wrong fixes
`leaveOpen: true` không reset position; đổi serializer không sửa stream; retry endpoint có thể nhân đôi side effects; chỉ set Position khi chưa bật buffering có thể lỗi trên non-seekable stream.
## Production implications
Giới hạn body size, buffering threshold, tránh log secrets/PII, redaction, temporary files, latency và cancellation.
## Senior engineer should notice
Middleware ordering và ownership của stream là contract. Test với MemoryStream seekable chưa thay thế integration test trên HTTP server thật.
