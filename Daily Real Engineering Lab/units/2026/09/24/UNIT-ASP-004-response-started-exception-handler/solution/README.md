# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms
Failure path có thể gửi success status và một phần CSV trước khi exception xảy ra; error middleware không thể biến response đã commit thành JSON 500 sạch.

## Evidence
Header/body được flush trước operation có thể fail. Sau thời điểm đó `HttpResponse.HasStarted` là boundary quan trọng.

## Root cause
Endpoint commit HTTP response trước khi hoàn tất phần fallible preparation. Error handling phía ngoài pipeline chỉ có thể thay status/header khi response chưa bắt đầu.

## Why the fix works
Với report nhỏ/vừa trong lab, chuẩn bị nội dung fallible trước, sau đó mới set response và write payload. Nếu preparation fail, exception handler vẫn có thể tạo 500. Chỉ commit 200 khi report đã sẵn sàng.

## How to verify
`verify.ps1` chạy learner-edited `starter/`, kiểm tra success payload và failure status.

## Alternative fixes
Với report rất lớn không thể buffer, dùng async export job + object storage/status endpoint; hoặc streaming protocol có explicit framing/error semantics. Chọn theo kích thước, latency và memory budget.

## Wrong / Tempting Fixes
Catch exception rồi đổi `StatusCode=500` sau khi body đã flush không sửa được response đã commit. Tăng timeout không giải quyết lifecycle contract. Nuốt exception khiến client càng khó phân biệt complete/incomplete data.

## Production implications
Theo dõi aborted/incomplete downloads, correlation ID, content integrity, memory pressure khi buffer, và retry semantics của export.

## Trade-offs
Buffer-before-commit cho error semantics rõ nhưng dùng memory/latency; streaming giảm memory/time-to-first-byte nhưng sau commit không còn HTTP-level rollback.

## What a Senior engineer should notice
HTTP response commit là một irreversible boundary. Thiết kế phải đặt các operation có thể fail và side effects quanh boundary đó có chủ đích.