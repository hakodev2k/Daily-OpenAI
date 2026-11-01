# Reference Solution — spoiler

## 1. Symptoms
Body đúng nhưng `X-Request-Trace` thiếu; lỗi có thể xảy ra sau khi response đã được gửi.

## 2. Evidence
Probe gọi Kestrel thật qua loopback, kiểm tra status, JSON và header cho hai endpoint. Starter trả `OBSERVED_FAILURE`.

## 3. Root cause
Middleware ghi `Response.Headers` sau `await next()`. Endpoint đã gọi `WriteAsync`, khiến response bắt đầu và headers trở thành read-only.

## 4. Why fix works
Đăng ký `Response.OnStarting` trước khi gọi downstream. Callback chạy ngay trước khi Kestrel commit headers. Xem [Program.cs](Program.cs).

## 5. Verify
Chỉnh `starter/Program.cs` theo cách của bạn, chạy `./scripts/verify.ps1`: status 200, JSON khớp, mỗi response có đúng một `X-Request-Trace` không rỗng và hai giá trị khác nhau. Script không kiểm tra source trong `solution/`.

## 6. Alternative fixes
Gán header trước `await next()` cũng hợp lệ nếu giá trị có sẵn và downstream không ghi đè. `OnStarting` hữu ích khi cần quyết định sát thời điểm gửi.

## 7. Wrong / tempting fixes
Nuốt exception chỉ che triệu chứng. Retry/timeout không thay đổi response lifecycle. Tắt middleware làm mất correlation contract. Ghi header sau khi endpoint `WriteAsync` vẫn quá muộn.

## 8. Production implications
Streaming, SSE, file downloads và error handling càng cần tôn trọng thời điểm response bắt đầu. Không đưa PII vào trace header.

## 9. Trade-offs
`OnStarting` có callback per request và phải giữ ngắn, không block. Ghi header sớm đơn giản hơn nếu không cần quan sát downstream.

## 10. Senior engineering insight
Sau khi headers được gửi, exception handler không thể đáng tin cậy đổi status. Cần wire-level regression test, không chỉ mock `HttpContext`.
