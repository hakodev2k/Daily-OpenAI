# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Một direct client có network peer không nằm trong allowlist vẫn có thể làm authorization layer nhìn thấy effective client IP thuộc allowlist bằng cách gửi forwarding metadata.

## 2. Evidence

Starter có bốn cases. Case quan trọng là `direct-with-forwarded-header`: `networkPeer=203.0.113.50` nhưng effective identity trở thành `10.20.30.40`. Trong khi đó legitimate proxy case cũng cần giữ khả năng chuyển original client identity.

## 3. Root cause

Application coi forwarding metadata là trusted identity bất kể request thực sự đến từ đâu. Header là input do request mang theo; nó chỉ có giá trị như một security-relevant assertion khi application biết network peer là intermediary đáng tin cậy.

## 4. Why the fix works

Reference implementation chỉ chấp nhận forwarded client IP khi network peer đúng bằng trusted proxy. Direct clients không thể tự nâng quyền bằng cách tự khai báo header. Legitimate proxy vẫn có thể chuyển original client identity.

Trong ASP.NET Core production, nguyên tắc tương ứng là cấu hình `ForwardedHeadersMiddleware` với trust boundary rõ ràng như `KnownProxies` hoặc `KnownNetworks`, đặt middleware đúng vị trí trước logic phụ thuộc `RemoteIpAddress`, và không mở trust rộng hơn topology thực tế.

## 5. How to verify

Copy logic tương đương vào `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Tất cả bốn cases phải `PASS`. Đặc biệt:

- direct spoof case phải bị từ chối
- trusted proxy + allowlisted original client phải được phép
- trusted proxy + non-allowlisted client phải bị từ chối

## 6. Alternative fixes

Nếu IP allowlisting không phải requirement bắt buộc, một identity-based control như authenticated service/user identity thường rõ ràng và audit được tốt hơn. Ở network edge, reverse proxy cũng có thể strip/replace forwarding headers từ untrusted clients, nhưng application vẫn nên có trust model phù hợp với topology thay vì dựa hoàn toàn vào giả định ngầm.

## 7. Wrong or misleading fixes

- Chỉ đổi tên header: không tạo ra trust boundary.
- Tin mọi proxy/network để “đỡ lỗi production”: làm mất ý nghĩa của kiểm tra nguồn gửi.
- Chặn riêng một IP attacker đã thấy: chỉ xử lý một biểu hiện, không sửa identity trust model.
- Bỏ forwarding metadata hoàn toàn: có thể làm legitimate proxied traffic mất original client identity và phá policy đang cần nó.

## 8. Production implications

Topology thay đổi phải đi cùng cấu hình trust boundary. Load balancer, ingress, CDN hoặc proxy chain mới có thể làm policy sai theo cả hai hướng: bypass authorization hoặc reject traffic hợp lệ. Cần test deployment topology, không chỉ unit-test một helper function.

## 9. Trade-offs

IP-based authorization đơn giản nhưng phụ thuộc network topology, proxy configuration và source-address semantics. Identity-based authorization thường mạnh hơn về auditability nhưng yêu cầu authentication lifecycle. Defense-in-depth có thể dùng cả network restriction và authenticated identity cho operations endpoint nhạy cảm.

## 10. What a Senior engineer should notice

Security decision không nên dựa trên một giá trị chỉ vì framework đã parse nó thành `RemoteIpAddress`. Câu hỏi quan trọng là **ai đã chứng thực assertion đó, qua trust boundary nào, và topology nào làm assertion còn đáng tin**. Senior engineer cũng phải đưa topology/configuration vào regression tests và deployment review.
