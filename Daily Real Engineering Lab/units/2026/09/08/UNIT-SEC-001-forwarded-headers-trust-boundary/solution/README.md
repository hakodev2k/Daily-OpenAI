# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms
Một request trực tiếp có thể gửi `X-Forwarded-For: 10.x.x.x` và khiến application coi nó là request internal.

## 2. Evidence
Starter tự đọc `X-Forwarded-For` trước khi xác lập bất kỳ trust relationship nào với network hop đã gửi request. Header vì vậy chỉ là client-controlled input nhưng lại được dùng như security identity.

## 3. Root cause
Application trộn **transport metadata** với **trusted identity**. Nó tin forwarding header chỉ vì header tồn tại, thay vì chỉ chấp nhận forwarding information từ reverse proxy/network boundary đã được cấu hình là trusted.

## 4. Why the fix works
Một hướng sửa phù hợp là:

- không tự parse `X-Forwarded-For` trong authorization logic;
- cấu hình ASP.NET Core Forwarded Headers Middleware;
- khai báo rõ proxy/network nào được tin cậy;
- để middleware chuẩn hóa `HttpContext.Connection.RemoteIpAddress`;
- authorization chỉ đọc normalized connection identity.

Ví dụ khi proxy local `127.0.0.1` là boundary được tin cậy trong lab:

```csharp
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
    options.KnownProxies.Clear();
    options.KnownNetworks.Clear();
    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
    options.ForwardLimit = 1;
});

var app = builder.Build();
app.UseForwardedHeaders();
```

Trong production, trusted proxy/network phải phản ánh topology thật; không copy `127.0.0.1` nếu proxy thực tế ở địa chỉ khác.

Nếu application có thể nhận traffic trực tiếp từ Internet và từ proxy trên cùng listener, network architecture cần đảm bảo direct clients không thể giả làm trusted proxy hop. Security boundary không thể chỉ dựa vào một header convention.

## 5. How to verify
Chạy `verify.ps1`. Một request trực tiếp có client-controlled forwarding metadata không được tự nâng thành internal classification.

Sau đó, trong môi trường proxy thực tế, thêm integration test chứng minh forwarding metadata từ **trusted proxy path** vẫn được xử lý đúng.

## 6. Alternative fixes
- Tách internal operations sang listener/network riêng và enforce bằng firewall/private ingress.
- Dùng authenticated service identity hoặc authorization policy thay cho IP allow-list khi phù hợp.
- Đặt administrative endpoint sau API gateway/reverse proxy có authentication mạnh.

## 7. Wrong or misleading fixes
- Chỉ validate `X-Forwarded-For` có đúng format IP: attacker vẫn gửi IP hợp lệ.
- Tin mọi private IP trong header: private-looking value không chứng minh request đến từ private network.
- Tin mọi proxy bằng cách bỏ giới hạn trusted proxies/networks: mở rộng trust boundary quá mức.
- Chỉ đổi tên header: không thay đổi ownership/trust semantics.

## 8. Production implications
Sai trust boundary có thể biến routing metadata thành authorization bypass. Ngoài endpoint quản trị, lỗi tương tự có thể ảnh hưởng rate limiting, audit attribution, geo policy và abuse detection.

## 9. Trade-offs
IP-based trust đơn giản nhưng phụ thuộc mạnh vào topology và proxy configuration. Authenticated workload identity phức tạp hơn nhưng thường rõ security semantics hơn. Network isolation mạnh nhưng tăng deployment/operations work.

## 10. What a Senior engineer should notice
Một giá trị không trở thành trusted chỉ vì infrastructure thường tạo ra nó. Senior engineer phải hỏi **ai có thể control input tại boundary hiện tại**, trust được thiết lập ở hop nào, framework normalize dữ liệu ra sao và authorization đang dựa vào evidence nào.
