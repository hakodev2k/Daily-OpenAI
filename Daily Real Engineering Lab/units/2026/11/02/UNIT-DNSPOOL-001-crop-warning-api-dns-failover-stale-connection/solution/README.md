# Reference Solution — chỉ xem sau khi tự thử

## 1. Symptoms
Sau cutover, `dnsActive=west` nhưng `connected=east` và response 503 lặp lại.

## 2. Evidence
Starter giữ một connection từ t=0 tới t=180, `dnsLookups=1`. Hai request trước cutover thành công; năm request sau cutover thất bại.

## 3. Root cause
Mô phỏng chỉ thực hiện DNS resolution khi tạo connection mới; connection ban đầu được tái sử dụng không giới hạn tuổi. DNS TTL không tự đóng connection đang mở. Đây là mô hình đơn giản, không phải DNS stack thực.

## 4. Why the fix works
Reference đặt `ConnectionPolicySeconds=45` trong `solution/Program.cs`. Tại t=61 cần connection mới; DNS cache t=0 đã hết hạn, nên connection được mở tới west. Các request tiếp theo tái sử dụng connection.

## 5. Verification
```powershell
./reproduce.ps1 # chạy trên starter chưa sửa
# tự sửa starter/Program.cs
./verify.ps1    # chạy trên starter bạn đã sửa
dotnet run --project ./solution/DnsPoolLab.csproj -- --verify
```
Reference đạt 2/2 trước cutover, 5/5 sau cutover, 0 lỗi sau cutover, 4 connections và 4 DNS lookups.

## 6. Alternatives
- Bounded pooled connection age: dùng `SocketsHttpHandler.PooledConnectionLifetime` trong ứng dụng thật; điều chỉnh theo failover budget và overhead.
- `IHttpClientFactory`: quản lý handler lifetime; phân biệt `HandlerLifetime` và pooled connection lifetime, không giữ typed client sai vòng đời.
- HttpClient mới mỗi request: không nên là mặc định vì socket/handshake overhead.
- Chủ động rollover khi failover: phục hồi nhanh nhưng phức tạp hơn; bảo vệ in-flight requests và retry.

## 7. Wrong / tempting fixes
- Chỉ giảm DNS TTL: không buộc connection đang mở đổi đích.
- Retry 503 vô hạn: vẫn nhắm vùng cũ và có thể nhân side effects.
- New HttpClient mỗi request: tạo churn không cần thiết.
- Chỉ scale out server: không giải quyết client connection lifecycle.

## 8. Production implications
Theo dõi endpoint thực, connection age, DNS resolution, retry rate, error rate và TLS handshake. Connection lifetime không hủy request đang chạy; cân nhắc request timeout, health checks, circuit breaker và idempotency.

## 9. Trade-offs
Lifetime ngắn tăng khả năng refresh đích khi tạo connection nhưng tăng TLS/socket overhead. Lifetime dài tiết kiệm tài nguyên nhưng có thể kéo dài phục hồi. 45 giây là giá trị cho trace, không phải default production.

## 10. What a Senior engineer should notice
Tách DNS answer, resolver cache, pooled TCP connection và application retry. Kiểm thử DNS/HTTP integration thật trước rollout; kiểm tra worst-case recovery, không chỉ trace thuận lợi. Official docs: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
