# Reference Solution — Chỉ xem sau khi tự điều tra

## 1. Symptoms
Sau khi audit middleware chạy, JSON hợp lệ trả 400 / invalid-json; auditBytes vẫn chính xác.

## 2. Evidence
NonSeekableReadStream mô phỏng request body forward-only. Audit đã đọc toàn bộ bytes trước endpoint. DeserializeAsync nhận EOF, không phải payload gốc.

## 3. Root cause
Request.Body là Stream có trạng thái tiêu thụ. Middleware đọc hết stream trước khi gọi RequestDelegate kế tiếp; endpoint không thể đọc lại từ stream không hỗ trợ Seek.

## 4. Why the fix works
Solution/RequestAuditMiddleware.cs gọi EnableBuffering với ngưỡng 30 KiB, giới hạn 1 MiB, đọc body để audit, rồi đặt Body.Position = 0 trước _next. Payload trở lại cho endpoint; audit vẫn hoạt động. Giới hạn demo phải được ánh xạ thành HTTP 413 hoặc policy phù hợp trong production.

## 5. How to verify
~~~powershell
dotnet run --project solution/AuditLab.Solution.csproj -- verify
~~~
Hai request hợp lệ trả 202 với body accepted:..., malformed/empty vẫn 400 và auditBytes không đổi. Verify.ps1 chỉ kiểm tra starter do người học chỉnh sửa.

## 6. Alternative fixes
- Không đọc body nếu chỉ cần metadata sẵn có và ContentLength có thể tin cậy.
- Parse body một lần ở ranh giới rõ ràng, chuyển typed object qua pipeline (đánh đổi coupling).
- Với upload lớn, thiết kế bounded streaming/tee thay vì buffering toàn bộ.

## 7. Wrong / tempting fixes
- Bỏ audit: mất yêu cầu nghiệp vụ.
- Bỏ JSON validation: che lỗi, làm sai contract.
- Chỉ đặt Position=0 trên stream gốc: stream forward-only có thể throw NotSupportedException.
- Chỉ EnableBuffering nhưng không rewind: downstream vẫn đọc EOF.
- Log toàn bộ raw body: rủi ro PII/credentials, memory và log cost.

## 8. Production implications
Test dùng MemoryStream seekable có thể che lỗi. Buffering có thể spill ra disk; cần request-size limit, cancellation, xử lý IOException/413, cleanup, redaction, telemetry và regression với Kestrel thật. Không nên dùng buffering cho upload lớn mà không có policy.

## 9. Trade-offs
Buffering dễ tích hợp nhưng tốn RAM/disk/I/O; parse-once giảm copy nhưng tăng coupling. Giải pháp đúng tùy payload size, middleware composition và API contract.

## 10. Senior engineer should notice
400 không đủ chứng minh payload client sai. Điều tra ownership/lifecycle của stream, phân biệt bằng chứng từng pipeline stage, bảo vệ negative tests, và mô phỏng forward-only stream để tránh false confidence.
