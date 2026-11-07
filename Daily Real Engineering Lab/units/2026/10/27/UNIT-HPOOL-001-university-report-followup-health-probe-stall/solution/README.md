# Reference Solution — SPOILER

Chỉ đọc sau khi tự reproduce, thu thập evidence và thử sửa.

## Symptoms
GET /report trả 200 headers, GET /health timeout. Fake partner ghi reportRequests=1, healthRequests=0; health chưa hề tới origin.

## Root cause
HttpCompletionOption.ResponseHeadersRead trả về khi có headers, không chờ hết body. Fixture giữ report body chưa kết thúc. Response report không được Dispose, MaxConnectionsPerServer=1 khiến request health bị queue cho đến khi deadline 900ms hết.

## Why the fix works
Xem [PartnerClient.cs](PartnerClient.cs). Dùng using block bao quanh report response, kết thúc trước health request. Vì chỉ cần headers, Dispose đóng body chưa dùng, giải phóng capacity cho handler. Với body chưa đọc hết, transport có thể đóng socket cũ và mở socket mới.

## Verification
Chạy reproduce.ps1 với starter gốc, sửa starter/PartnerClient.cs rồi chạy verify.ps1. PASS yêu cầu reportRequests=1, healthRequests=1, body PONG. verify chạy learner-editable starter, không chạy solution.

## Alternative fixes
Nếu partner có HEAD endpoint, ưu tiên HEAD cho metadata. Nếu cần body, stream và giới hạn bytes/time rồi Dispose trong finally/using. Có thể tăng MaxConnectionsPerServer nếu capacity thực tế cần, nhưng không thay thế response lifecycle ownership.

## Wrong fixes
Tăng timeout chỉ kéo dài queue. Tạo HttpClient mới cho từng request gây connection churn. Ép GC không thay thế deterministic disposal. Cố đọc toàn bộ body không giới hạn có thể treo vì fixture không kết thúc.

## Production implications
Instrument connection queue wait, time-to-headers, body-read duration và active outbound requests. ResponseHeadersRead cho phép streaming nhưng caller phải quản lý response/body. Drain bounded có thể giữ socket reuse; abort body hợp lý khi chỉ cần headers.

## Senior engineer should notice
Headers received khác request resources released. Pool capacity, backpressure và cancellation phải được xem xét cùng nhau. Regression test phải chứng minh request health thực sự tới origin.

## Official references
- https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
- https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcompletionoption
- https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage.dispose
