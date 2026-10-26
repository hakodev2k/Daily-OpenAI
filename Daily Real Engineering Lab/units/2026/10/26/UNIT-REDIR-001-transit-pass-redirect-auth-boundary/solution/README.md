# Reference Solution — SPOILER

**Chỉ xem sau khi tự reproduce, tự đưa giả thuyết và chạy verify.**

## Symptoms
Direct partner validation trả 200, qua gateway 307 rồi 401. Gateway nhận token; endpoint đích được gọi nhưng không xác thực thành công.

## Evidence
Starter harness quan sát direct 200; gateway path trả 401; `secure_requests=1`, `authorized=0` trong reproduction. Response đầu từ gateway là 307 tới một path cùng origin. Bằng chứng loại trừ giả thuyết service không chạy và credential sai ở request đầu.

## Root cause
`HttpClientHandler` với `AllowAutoRedirect=true` tự theo HTTP redirect và **xóa Authorization header** trước request tiếp theo. Đường direct không chuyển hướng nên vẫn được xác thực. Đây là chính sách an toàn của HTTP client, không phải lỗi partner 401 ngẫu nhiên.

## Fix và vì sao hoạt động
Tham khảo [reference code](PartnerGatewayClient.cs). Tắt auto redirect, nhận status 3xx, kiểm tra `Location`, resolve URI, so sánh scheme/host/port với origin đã tin cậy, giới hạn số hop, tạo request mới cho mỗi hop và chỉ khi hợp lệ mới gắn Authorization. Không forward credential sang origin khác. Bản reference áp dụng policy chỉ cùng origin và GET; với production, trust policy cần cấu hình theo hợp đồng partner.

## How to verify
Sau khi tự sửa, chạy `scripts/verify.ps1`. Để đối chiếu reference, sao chép `solution/PartnerGatewayClient.cs` đè `starter/PartnerGatewayClient.cs` **chỉ sau khi tự thử**. Verify kiểm tra 200 ở direct và redirected path, đủ hai request có auth, và external redirect bị từ chối bằng exception chính sách. Reproduce lúc này phải fail (exit 1) vì không còn symptom.

## Alternative fixes
- Gọi trực tiếp endpoint ổn định của partner nếu hợp đồng đảm bảo không redirect; đơn giản hơn nhưng phụ thuộc topology/versioning.
- Cho phép redirect tới allowlist origin đã kiểm định với token audience riêng; không tự forward cùng bearer token qua các host.
- Nếu partner hỗ trợ challenge-based auth, cấu hình credential mechanism phù hợp; kiểm tra kỹ scope của challenge và redirects.

## Wrong / tempting fixes
- Tăng retry hoặc timeout: 401 deterministic sau redirect không phải transient 503.
- Tắt xác thực ở service đích: phá security boundary.
- Đưa bearer token vào query string hoặc redirect URL: có thể rò rỉ qua logs, browser history, proxy.
- Gửi Authorization vô điều kiện tới mọi Location: có thể exfiltrate credential sang domain do đối tác hoặc attacker điều khiển.
- Chỉ chấp nhận 401 như kết quả nghiệp vụ hợp lệ: che giấu mất chức năng kiểm tra vé.

## Production implications
Quan sát status của từng hop, redirect count, origin category và correlation ID; redact Authorization. Định nghĩa chính sách HTTPS, allowlist, DNS/redirect governance, giới hạn hop và timeout end-to-end. Token audience và trust domain phải được xét riêng; same-origin không tự động chứng minh endpoint là an toàn trong mọi topology.

## Trade-offs
Manual redirect tăng code và trách nhiệm kiểm soát security policy, nhưng bảo đảm behavior minh bạch. Direct endpoint ít phức tạp hơn nếu ổn định. Tự động redirect thuận tiện với request không có credential nhạy cảm nhưng không đáp ứng hợp đồng auth của tình huống này.

## What a Senior engineer should notice
Sự cố authentication có thể do client policy trên từng network hop, không chỉ do token issuance/expiry. Regression test cần kiểm tra cả functional 200 và security invariant không phát tán token sang origin ngoài trust boundary. Chỉ dùng static-review trong lần tạo lab này; chưa xác nhận bằng thực thi.
