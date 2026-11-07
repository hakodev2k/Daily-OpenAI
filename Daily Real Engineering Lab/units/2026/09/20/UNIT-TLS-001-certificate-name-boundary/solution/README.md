# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
TCP route tới endpoint mới reachable, certificate còn hạn và trusted, nhưng secure connection dừng trước HTTP.

## 2. Evidence
Client gọi `https://10.24.8.17/...`; certificate chỉ công bố DNS identity `partner-api.internal.example`. Browser dùng hostname chuẩn nên không gặp lỗi.

## 3. Root cause
TLS server identity validation so khớp host mà client yêu cầu với identity trong certificate SAN. IP literal không phải identity được certificate này xác nhận.

## 4. Why the fix works
Đổi `starter/client.json` để endpoint dùng `https://partner-api.internal.example/orders` làm request identity khớp SAN. Trong production, DNS của hostname đó phải route tới endpoint mới.

## 5. How to verify
Chạy `./verify.ps1`. Simulator phải in `RESULT=HTTPS_REQUEST_CAN_PROCEED` và `VERIFY_PASS`.

## 6. Alternative fixes
Nếu requirement bắt buộc truy cập bằng IP, certificate phải được phát hành với IP SAN phù hợp. Thông thường giữ stable DNS name và thay đổi DNS/routing khi migration dễ vận hành hơn.

## 7. Wrong or misleading fixes
Không disable certificate validation hoặc chấp nhận mọi certificate callback. Việc đó xóa authentication property của TLS. Đổi firewall hoặc tăng timeout cũng không giải quyết identity mismatch khi TCP đã reachable.

## 8. Production implications
Endpoint migration cần coi DNS name, certificate SAN, SNI, proxy routing và client URL là một contract. Health check chỉ kiểm TCP có thể báo xanh dù HTTPS application path vẫn hỏng.

## 9. Trade-offs
Stable DNS abstraction thêm dependency vào DNS nhưng giảm coupling client với IP và hỗ trợ rotation/migration. IP SAN có thể phù hợp với một số môi trường cố định nhưng làm lifecycle certificate và endpoint chặt hơn.

## 10. What a Senior engineer should notice
Phải định vị failure theo layer trước khi sửa. “Network reachable” không đồng nghĩa “HTTPS identity valid”; certificate trust, validity và hostname identity là các kiểm tra khác nhau.