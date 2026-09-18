# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Absolute recovery URL có thể mang hostname do một request không thuộc trusted proxy path cung cấp.

## 2. Evidence
Request trực tiếp từ địa chỉ ngoài proxy vẫn có forwarding-related value và application dùng value đó để tạo public origin.

## 3. Root cause
Code coi `ForwardedHost` là authoritative chỉ vì header/value tồn tại. Forwarding metadata là dữ liệu có trust boundary; nếu application không giới hạn nguồn proxy được tin cậy, client có thể ảnh hưởng host dùng cho security-sensitive absolute URL.

## 4. Why the fix works
Chỉ chấp nhận forwarded host khi request đến từ proxy đã cấu hình là trusted, đồng thời giới hạn public host theo deployment contract. Với simulator này, một implementation tối thiểu có thể kiểm tra trusted proxy address trước khi dùng forwarded value và fallback về configured public origin cho request khác.

Ví dụ:
```csharp
private const string PublicHost = "accounts.example.test";
private static readonly HashSet<string> TrustedProxies = ["10.0.0.10"];

public static string Build(RequestEnvelope request, string token)
{
    var host = TrustedProxies.Contains(request.RemoteAddress)
        && string.Equals(request.ForwardedHost, PublicHost, StringComparison.OrdinalIgnoreCase)
        ? request.ForwardedHost
        : PublicHost;

    return $"https://{host}/account/recover?token={Uri.EscapeDataString(token)}";
}
```

Trong ASP.NET Core production, cấu hình `ForwardedHeadersMiddleware` với known proxies/networks phù hợp topology và áp dụng host validation/configured public origin theo deployment contract.

## 5. How to verify
Chạy `./verify.ps1`. Legitimate proxy request phải tạo URL dưới `accounts.example.test`; direct request không được đưa hostname tùy ý vào URL.

## 6. Alternative fixes
Dùng configured canonical public origin cho toàn bộ recovery links là lựa chọn đơn giản và mạnh khi application chỉ có một public origin. Multi-tenant systems có thể cần allow-list theo tenant thay vì một host cố định.

## 7. Wrong or misleading fixes
Xóa forwarding support hoàn toàn có thể phá deployment sau proxy. Chỉ kiểm tra header có tồn tại không không tạo trust. Encode hostname không giải quyết authority problem. Tin mọi proxy để “fix production” làm trust boundary rộng hơn.

## 8. Production implications
Sai trust boundary ở absolute URL generation có thể ảnh hưởng password recovery, OAuth redirect construction, canonical links và notification links.

## 9. Trade-offs
Configured origin đơn giản và dễ audit nhưng kém linh hoạt. Dynamic origin hỗ trợ nhiều host nhưng cần mapping, validation và proxy topology rõ ràng.

## 10. What a Senior engineer should notice
Security property nằm ở boundary giữa infrastructure và application, không chỉ ở string manipulation. Cần kiểm tra cả proxy configuration, host allow-list, deployment topology và regression tests cho direct/untrusted traffic.