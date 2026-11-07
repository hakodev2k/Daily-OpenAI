# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Hai `HttpClient` object được tạo riêng nhưng call thứ hai tự động gửi `LegacySession=tenant-a`.

## Evidence

`CreateClient` trả về client mới, nhưng transport handler có thể được factory pool và tái sử dụng. Cookie state thuộc handler-side `CookieContainer`, không thuộc riêng client object.

## Root cause

Named client dùng pooled `HttpMessageHandler` với automatic cookie handling. `CookieContainer` của handler giữ cookie nhận từ call trước và tự động gắn nó vào call sau trong cùng handler lifetime. Trong gateway phục vụ nhiều tenant, đó là state boundary sai.

## Why the fix works

Không để pooled transport sở hữu session cookie state. Đặt `UseCookies = false` và, nếu integration thực sự cần cookie, truyền cookie một cách explicit theo đúng request/tenant/session boundary.

Ví dụ cấu hình:

```csharp
builder.Services.AddHttpClient("partner", c =>
    c.BaseAddress = new Uri("http://127.0.0.1:5098"))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = false
    });
```

Sau đó cookie cần thiết phải được lấy từ state thuộc đúng tenant/session và thêm explicit vào request thay vì dựa vào handler-global cookie jar.

## How to verify

Chạy `verify.ps1`. Call đầu vẫn phải thành công và call thứ hai phải báo `SECOND_CALL_COOKIE=<none>`.

## Alternative fixes

Một handler/client riêng cho từng session có thể phù hợp trong integration đặc thù, nhưng cần quản lý connection reuse và lifetime cẩn thận. Một explicit per-session cookie store cũng hợp lý nếu legacy protocol bắt buộc session cookie.

## Wrong or misleading fixes

- Tạo `HttpClient` mới bằng `IHttpClientFactory` cho từng call: không đảm bảo handler state mới.
- Giảm `HandlerLifetime` cực thấp: chỉ làm leak ít deterministic hơn và làm xấu connection reuse; không thiết lập đúng state boundary.
- Clear một shared `CookieContainer` trước mỗi request: dễ tạo race khi concurrent requests dùng cùng handler.
- Bỏ `IHttpClientFactory` và `new HttpClient()` mỗi call: có thể tránh symptom nhưng đánh đổi connection management và giải quyết sai abstraction boundary.

## Production implications

Cookie/session state trong shared outbound HTTP infrastructure có thể trở thành cross-user hoặc cross-tenant data leak. Cần audit cả authentication headers, delegating handlers và mutable per-request state được giữ ở lifetime dài.

## Trade-offs

Disabling automatic cookies làm code integration explicit hơn nhưng tăng trách nhiệm quản lý session state. Đổi lại, ownership và isolation boundary rõ ràng, đặc biệt quan trọng với multi-tenant services.

## What a Senior engineer should notice

Object lifetime ở application layer không đồng nghĩa resource/state lifetime ở transport layer. Khi dùng factory/pooling, luôn xác định state nào được pool, state nào per-request và state nào có thể vượt qua security/tenant boundary.
