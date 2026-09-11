# Reference Solution — UNIT-HTTP-005

> Chỉ xem sau khi đã reproduce và thử fix trong `starter/`.

## 1. Symptoms

Process dài hạn nhận biết endpoint registry đã đổi từ backend A sang B, nhưng request tiếp theo vẫn được backend A xử lý.

## 2. Evidence

- Request đầu tiên: `X-Backend: A`.
- Registry được đổi sang port của B.
- Request thứ hai vẫn trả `X-Backend: A`.
- Không có lỗi DNS/connect; connection cũ vẫn hợp lệ và được reuse.

## 3. Root cause

`HttpClient` dùng `SocketsHttpHandler` với pooled connection không có lifetime hữu hạn. Việc resolve/chọn endpoint xảy ra khi tạo **connection mới**, không phải trước mọi logical HTTP request. Khi một keep-alive connection tới A còn reusable, request sau có thể tiếp tục đi trên connection đó dù nguồn endpoint discovery đã đổi sang B.

## 4. Why the fix works

Reference solution cấu hình `PooledConnectionLifetime` hữu hạn. Sau khi connection vượt lifetime, handler không reuse nó cho request mới; request tiếp theo tạo connection khác và `ConnectCallback` đọc endpoint registry hiện tại, lúc đó là B.

Điểm quan trọng: vẫn reuse `HttpClient`; ta quản lý vòng đời connection pool chứ không phá pooling ở cấp request.

## 5. How to verify

Copy logic tương đương từ `solution/ClientFactory.cs` sang `starter/ClientFactory.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Expected:

```text
request-1 backend=A registry=A
registry switched to B
request-2 backend=B registry=B
```

## 6. Alternative fixes

- Với `IHttpClientFactory`, có thể quản lý handler lifetime phù hợp với DNS/service-discovery behavior của môi trường.
- Một custom connection strategy có thể chủ động invalidate pool khi control plane phát endpoint-change event, nếu hệ thống thực sự cần reaction nhanh hơn periodic refresh.
- Nếu protocol/load balancer đảm bảo connection drain khi rotate endpoint, operational policy cũng có thể giảm nhu cầu refresh ngắn; vẫn phải đo thực tế.

## 7. Wrong or misleading fixes

### Tạo `new HttpClient()` cho mọi request

Có thể khiến endpoint được resolve thường xuyên hơn nhưng phá connection reuse, tăng socket/TLS overhead và có thể gây resource pressure. Đây không phải default fix.

### Retry cùng client ngay lập tức

Nếu request cũ vẫn thành công, retry không được kích hoạt. Nếu connection vẫn reusable, retry cũng có thể tiếp tục dùng cùng endpoint.

### Giảm DNS TTL nhưng không thay connection policy

TTL chỉ ảnh hưởng resolution khi có resolution mới. Một pooled connection đang sống không cần resolve DNS lại.

## 8. Production implications

- Endpoint rotation, Kubernetes service changes, blue/green deployment và DNS failover đều có thể bị ảnh hưởng bởi connection lifetime.
- Lifetime quá dài làm process chậm thích nghi với topology change.
- Lifetime quá ngắn làm tăng reconnect, TCP/TLS handshake và CPU/network overhead.

## 9. Trade-offs

Chọn lifetime dựa trên DNS/service-discovery TTL, deployment behavior, load balancer drain policy, connection setup cost và yêu cầu failover. Không có một con số đúng cho mọi hệ thống.

## 10. What a Senior engineer should notice

Senior engineer phải phân biệt **lifetime của `HttpClient`**, **lifetime của handler** và **lifetime của pooled physical connection**. “Reuse HttpClient” là lời khuyên đúng nhưng chưa đủ; connection refresh policy phải phù hợp với topology động của production.