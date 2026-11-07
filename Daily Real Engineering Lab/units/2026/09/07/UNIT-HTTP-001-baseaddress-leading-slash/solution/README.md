# Reference Solution — Spoiler

`HttpClient` resolves the request URI using standard URI-reference semantics. A request target beginning with `/` is root-relative, so it replaces the path component of `BaseAddress` instead of appending beneath it.

Với:

```text
BaseAddress    = https://fulfillment.local/gateway/v1/
Request target = /orders/42
```

URI cuối cùng trở thành:

```text
https://fulfillment.local/orders/42
```

Fix phù hợp cho contract hiện tại là giữ trailing slash của `BaseAddress` và dùng relative target không có leading slash:

```csharp
var requestTarget = "orders/42";
var response = await client.GetAsync(requestTarget);
```

Khi đó URI cuối cùng là:

```text
https://fulfillment.local/gateway/v1/orders/42
```

## Root cause

Bug không nằm ở DNS, reverse proxy hay route registration. Nó nằm ở URI composition semantics giữa `BaseAddress` và request target.

## Trade-offs

- Centralized typed/named `HttpClient` configuration giúp tránh mỗi call site tự ghép URL khác nhau.
- Absolute request URI có thể hợp lệ nhưng dễ bypass intended base path/configuration nếu dùng tùy tiện.
- String concatenation thủ công dễ tạo double slash, thiếu escaping hoặc xử lý query không đúng; nên để `Uri`/`HttpClient` thực hiện resolution theo contract rõ ràng.
- Integration test nên assert request URI ở handler boundary, không chỉ mock `GetAsync` trả 200.
