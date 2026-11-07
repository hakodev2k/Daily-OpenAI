# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms
Audit ghi đủ payload nhưng endpoint trả 422 cho request có `permitId`.

## Evidence
Starter: `valid-permit expected=202 actual=422`; `missing-permit expected=422 actual=422`. Middleware đọc được bytes; endpoint đọc chuỗi rỗng.

## Root cause
Request body là stream có vị trí đọc. Middleware đọc đến EOF rồi chuyển tiếp mà không đưa vị trí về đầu. Downstream đọc từ vị trí EOF.

## Why fix works
Bật `EnableBuffering` trước khi đọc; đọc để audit rồi đặt `Body.Position = 0` trước khi gọi next. Giới hạn `bufferLimit` tránh buffering không kiểm soát; trong production cần thiết kế xử lý payload vượt ngưỡng và bảo vệ PII.

## Verification
Chép thay đổi vào `starter/Program.cs`, chạy `./verify.ps1`; hai case PASS và exit 0. `./reproduce.ps1` luôn chạy original starter (từ Git nếu có, xem script) nên chỉ dùng trước khi sửa.

## Alternatives
Đặt audit sau endpoint nếu nghiệp vụ cho phép, hoặc chỉ audit metadata/hash thay vì toàn body; cân nhắc middleware/endpoint filters và request-size limit.

## Wrong / tempting fixes
Không thay validation để coi empty body là hợp lệ; không cache payload bằng string vô hạn; không đọc lại body bằng stream khác mà thiếu lifecycle/size policy.

## Production implications
Body có thể lớn, chứa thông tin nhạy cảm, hoặc bị client hủy giữa chừng. Buffering có thể spill ra disk; giới hạn size, log redaction, cancellation và exception handling cần được thiết kế.

## Trade-offs
Buffering tăng memory/disk I/O và latency; audit hash/metadata giảm rủi ro nhưng mất forensic payload. Giới hạn 64 KiB chỉ là giả định cho harness.

## Senior engineer should notice
Phải test pipeline nhiều middleware và các request body lớn, invalid JSON, cancellation, PII; xác định owner của stream và hợp đồng rewind thay vì chỉ vá symptom.
