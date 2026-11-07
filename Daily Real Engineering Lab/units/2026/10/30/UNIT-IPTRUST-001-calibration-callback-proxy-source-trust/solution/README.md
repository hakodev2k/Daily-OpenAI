# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce và tự đề xuất fix. File `solution/GatewayAuthorizer.cs` là reference độc lập; `verify.ps1` không biên dịch file này.

## 1. Triệu chứng
Một direct connection từ `198.51.100.77` mang `X-Forwarded-For: 203.0.113.10` được starter cho phép. Request từ đúng partner nhưng đi trực tiếp cũng có thể được chấp nhận, trái hợp đồng một-hop.

## 2. Bằng chứng
Fixture `spoofed-direct` có remote TCP peer ngoài tập proxy tin cậy nhưng header tự nhận là partner. Fixture `valid-proxy-partner` đi qua `10.0.0.10` mới là luồng hợp lệ. `proxy-multiple-hops` cho thấy starter bỏ qua phần còn lại của chain.

## 3. Root cause
Starter tin dữ liệu `X-Forwarded-For` trước khi xác thực peer gửi header. HTTP header do caller tự cung cấp không phải chứng cứ danh tính nguồn; ở đây trust boundary của reverse proxy bị bỏ qua.

## 4. Vì sao fix hoạt động
Reference kiểm tra `remotePeer == 10.0.0.10` trước, yêu cầu đúng một forwarded IP có cú pháp hợp lệ, rồi so sánh với allowlist bằng `IPAddress.Equals`. Direct request, header rỗng, chain nhiều hop và địa chỉ ngoài allowlist đều fail closed.

## 5. Verify
Chỉ sửa `starter/GatewayAuthorizer.cs` theo chính sách đã phân tích; chạy `./verify.ps1` và yêu cầu `mismatches=0`. Chạy `./reproduce.ps1` sau khi fix phải **không còn** tái hiện lỗi (exit code 1). Có thể đối chiếu reference sau khi hoàn tất.

## 6. Alternative fixes
Ở ASP.NET Core thật, đặt proxy trust policy tại hạ tầng và cấu hình `ForwardedHeadersMiddleware` với `KnownProxies`/`KnownNetworks`, `ForwardLimit` phù hợp và thứ tự middleware đúng. Đọc `HttpContext.Connection.RemoteIpAddress` sau khi xử lý trusted forwarding. Nếu tất cả request phải qua edge, chặn direct origin access bằng network ACL/mTLS, không chỉ dựa vào application logic.

## 7. Wrong / tempting fixes
- Lấy phần tử đầu của `X-Forwarded-For` mà không kiểm tra peer: vẫn có thể spoof.
- Tin header khi thấy IP thuộc allowlist: header là dữ liệu do request mang tới.
- Bật xử lý forwarded headers từ mọi network: mở rộng trust boundary không có kiểm soát.
- Chặn mọi header có dấu phẩy trong production đa-hop: phù hợp contract lab một-hop, nhưng có thể từ chối lưu lượng hợp lệ trong topology khác.

## 8. Production implications
Giữ danh sách proxy tin cậy theo cấu hình triển khai; đảm bảo proxy **xóa hoặc ghi đè** forwarded headers từ phía client, và origin không mở công khai. Ghi log riêng `RemoteIpAddress`, chain đã xác minh và quyết định policy; tránh lưu toàn bộ dữ liệu nhạy cảm. Có thể cần hỗ trợ IPv6, IPv4-mapped IPv6 và nhiều hop theo policy được thiết kế rõ ràng.

## 9. Trade-offs
Kiểm soát tại network edge giảm bề mặt tấn công nhưng đòi hỏi phối hợp vận hành; kiểm soát trong app bổ sung defense-in-depth. Multi-hop cần định nghĩa chính xác các proxy tin cậy và chiều duyệt chain; không thể suy diễn từ chuỗi header.

## 10. Senior engineer nên nhận ra
Không nhầm IP trong HTTP header với IP của connection peer; mô tả topology, trust boundary và policy trước khi viết parser. Thử cả negative cases, không chỉ happy path. Đừng coi IP allowlist là cơ chế xác thực duy nhất cho callback có giá trị cao.

## Tham khảo
- Microsoft Learn: [Configure ASP.NET Core to work with proxy servers and load balancers](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer)
- Microsoft Learn: [Forwarded Headers Middleware](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forwarded-headers-middleware-options)
