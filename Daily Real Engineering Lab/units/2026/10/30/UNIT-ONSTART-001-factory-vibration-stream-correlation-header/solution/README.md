# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Health trả 204 và có `X-Trace-Id`, nhưng luồng SSE trả 200 với payload đúng mà thiếu header.

## Evidence
`reproduce.ps1` kiểm tra cùng một app, cùng middleware, khác response behavior. Route health không ghi body; route SSE ghi và flush body. Kiểm tra `HasStarted` sau `next()` cho thấy khác biệt.

## Root cause
Middleware chỉ thêm header **sau** `await next()` và chỉ khi `Response.HasStarted == false`. Route SSE đã ghi/flush response trước khi middleware tiếp tục; HTTP headers không thể được bổ sung sau thời điểm đó. Health không ghi body nên thường còn cơ hội bổ sung header.

## Why the fix works
Đăng ký `Response.OnStarting` **trước** khi gọi `next()`. Callback chạy khi server chuẩn bị gửi response headers, kể cả với endpoint streaming, nên header được thiết lập đúng lúc. Xem [FactoryDashboard.cs](FactoryDashboard.cs). Sao chép nội dung file này sang `starter/FactoryDashboard.cs` nếu muốn thử reference.

## How to verify
Chạy `./verify.ps1` sau khi sửa starter. Harness khởi động Kestrel trên loopback với cổng ngẫu nhiên, gọi cả hai route, xác nhận status, SSE content và header. `verify.ps1` không chạy source trong thư mục solution.

## Alternative fixes
- Gắn header trước `await next()` khi giá trị đã biết; đây là cách đơn giản hơn nếu không cần đợi response metadata cuối cùng.
- Dùng middleware chuyên biệt cho request ID/trace context nếu ứng dụng có nhiều điểm tích hợp, nhưng phải kiểm tra contract trên streaming và error paths.

## Wrong or misleading fixes
- Bỏ điều kiện `HasStarted` rồi gắn header sau `next()`: có thể gây `InvalidOperationException` khi response đã bắt đầu.
- Thêm `Task.Delay` trước khi flush: chỉ làm lỗi phụ thuộc timing.
- Chuyển tất cả SSE thành buffered JSON: thay đổi API contract và latency.
- Bổ sung header chỉ trong SSE endpoint: che khuất thiếu sót ở middleware dùng chung.

## Production implications
Các endpoint streaming, file download, gRPC và response được flush sớm cần kiểm tra header contract độc lập. Correlation ID nên được sinh và quản lý nhất quán, không tin mù quáng ID do client cung cấp; không đưa dữ liệu nhạy cảm vào header/log.

## Trade-offs
`OnStarting` hỗ trợ quyết định muộn, nhưng callback cần gọn, không blocking và không được giả định có thể sửa headers sau khi response bắt đầu. Nếu header đã xác định trước, gắn sớm là đơn giản nhất.

## What a Senior engineer should notice
Một HTTP 200 và payload đúng chưa đủ chứng minh observability contract. Test nên chạy qua HTTP server thực và kiểm tra headers ở client, thay vì chỉ unit-test việc gọi middleware. Phân biệt việc response bắt đầu, endpoint hoàn tất và middleware unwind.
