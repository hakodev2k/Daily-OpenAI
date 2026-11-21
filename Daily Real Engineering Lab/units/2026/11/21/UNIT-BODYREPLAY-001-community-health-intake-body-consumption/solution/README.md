# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms
Audit thấy JSON; handler nhận body trống.

## Evidence
Cả ba trường hợp có `auditChars > 0`, `received = 0`.

## Root cause
Request.Body là stream có cursor. Audit đọc đến cuối rồi chuyển cho handler mà không khôi phục vị trí.

## Why the fix works
`EnableBuffering()` cho phép replay request body trên hosting thực; `leaveOpen: true` giữ stream và `Position = 0` trước `next` giúp handler đọc từ đầu. Reference nằm tại `solution/Program.cs`.

## How to verify
Sửa `starter/Program.cs`, chạy `./scripts/verify.ps1`; script kiểm tra learner-editable starter, không kiểm tra riêng reference.

## Alternative fixes
Audit metadata thay vì payload; capture có giới hạn kích thước và chuyển immutable representation.

## Wrong or misleading fixes
Tăng timeout không sửa stream cursor; mở reader mới cũng không tự rewind; tắt audit chỉ che lỗi.

## Production implications
Giới hạn body, tránh log PII, kiểm soát buffer memory/disk, audit retention, exception handling.

## Trade-offs
Buffering tăng allocation và I/O; cần đánh giá trước khi bật trên mọi request.

## What a Senior engineer should notice
Ownership, middleware order, disposal, security boundary, payload size, test Unicode và multi-request.
