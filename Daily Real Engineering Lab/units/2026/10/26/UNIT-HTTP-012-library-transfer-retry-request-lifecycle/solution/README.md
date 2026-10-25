# Reference Solution — SPOILER

**Chỉ xem sau khi tự reproduce, điều tra và verify.**

## Symptoms
Partner trả 503 cho lần gửi đầu. Worker cố retry nhưng ném InvalidOperationException; fake partner chỉ ghi nhận một lần nhận request.

## Evidence
Harness quan sát được 1 outbound call dù cấu hình 2 attempts. Body, path và Idempotency-Key của attempt đầu hợp lệ. Case 202 đơn lẻ thành công; lỗi chỉ xuất hiện khi đi qua retry branch.

## Root cause
HttpRequestMessage là một request object single-use đối với HttpClient.SendAsync. Starter tạo một request cùng HttpContent trước vòng lặp, rồi gửi lại cùng object ở lần thứ hai. HttpClient chặn lần gửi lại trước khi fake handler nhận request.

## Fix và vì sao hoạt động
Trong [reference code](TransferDispatcher.cs), tạo HttpRequestMessage và JsonContent **bên trong mỗi attempt**; dispose response và request sau attempt. TransferId là identity nghiệp vụ nên Idempotency-Key phải giữ nguyên trên tất cả attempts của cùng transfer. Cách này bảo toàn retry budget và payload.

## How to verify
Sao chép solution/TransferDispatcher.cs sang starter/TransferDispatcher.cs **chỉ sau khi tự làm**. Chạy scripts/verify.ps1. Harness xác nhận 503→202 có 2 calls và trả 202, 400 không retry, 503→503 dừng ở 2 calls; payload/path/key nhất quán. Reproduce sẽ không còn tái hiện lỗi và trả exit 1 sau khi sửa, theo đúng contract.

## Alternative fixes
- Đưa retry vào một resilience handler có factory tạo request mới cho mỗi attempt; phải chứng minh body có thể replay và policy chỉ retry status/transient phù hợp.
- Dùng command/outbox queue nếu nghiệp vụ cần durable retry qua process crash; phải bổ sung trạng thái, deadline và idempotency phía partner.
- Chỉ retry khi hợp đồng partner xác nhận Idempotency-Key có hiệu lực, nhất là với POST có side effects.

## Wrong / tempting fixes
- Chỉ thêm delay hoặc tăng số attempts: không làm object single-use trở nên replayable.
- Nuốt InvalidOperationException rồi coi như thành công: mất chuyển sách.
- Tạo key mới mỗi retry: có thể biến một transfer thành nhiều side effects.
- Retry mọi 4xx: thường là lỗi request không transient và tăng tải không cần thiết.
- Bật HttpClientHandler tự động retry tùy tiện: không mặc định bảo đảm POST replay/idempotency.

## Production implications
Log attempt number, response status, partner correlation ID, timeout và idempotency key (không log bí mật). Áp dụng backoff/jitter, bounded deadline, cancellation propagation và quan sát duplicate side effects. Xác định ranh giới retry khi response có thể đã bị mất sau khi partner xử lý thành công.

## Trade-offs
Retry tại service dễ hiểu nhưng lặp policy; resilience pipeline giúp nhất quán song cần cẩn thận về lifecycle của request body, auth context và side effects. Hai attempts là policy của lab, không phải cấu hình production mặc định.

## Senior engineer should notice
Một 503 không đồng nghĩa chắc chắn operation chưa được thực thi; retry POST cần business idempotency, replayable payload và end-to-end deadline. Chỉ test HTTP status là chưa đủ: phải assert số attempts, payload, key và no-retry cases. Validation của unit là static-review; code chưa được thực thi trong môi trường tạo lab.
