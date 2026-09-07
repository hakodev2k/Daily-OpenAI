# Reference Solution — Spoiler

Root cause là URI resolution semantics, không phải DNS hay network transport. Base address hiện tại kết thúc bằng path segment `api` nhưng không có trailing slash. Khi `HttpClient` resolve relative URI `orders/42`, segment cuối của base path bị thay thế, tạo request tới:

```text
https://example.test/orders/42
```

thay vì:

```text
https://example.test/api/orders/42
```

Một fix rõ ràng là biểu diễn base URI như một directory path:

```csharp
using var client = new HttpClient(handler)
{
    BaseAddress = new Uri("https://example.test/api/")
};

using var response = await client.GetAsync("orders/42");
```

Sau fix, URI cuối cùng là `https://example.test/api/orders/42`.

## Cơ chế

URI combination không phải string concatenation. Relative reference được resolve theo URI rules: base path không kết thúc bằng `/` được coi như có segment cuối có thể bị replace; relative path bắt đầu bằng `/` lại trở thành absolute path trên cùng authority và bỏ toàn bộ base path.

## Trade-offs

- Chuẩn hóa `BaseAddress` thành directory-style URI giúp call site dùng relative paths nhất quán.
- Có thể truyền absolute URI cho từng request nhưng sẽ làm mất lợi ích cấu hình endpoint tập trung và tăng duplication.
- Với `IHttpClientFactory`, nên validate base URI tại startup/configuration boundary và có integration test cho URI thực tế, không chỉ test chuỗi input riêng lẻ.
