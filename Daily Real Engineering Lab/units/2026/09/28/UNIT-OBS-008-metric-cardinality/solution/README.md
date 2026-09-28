# Reference Solution

## Root cause

Instrumentation dùng raw request path làm metric dimension. User ID nằm trong path nên mỗi user tạo một label value mới; 5.000 requests tạo 5.000 series.

## Fix tham chiếu

Đưa một bounded logical route vào instrumentation thay vì raw path:

```csharp
for (var userId = 1; userId <= 5000; userId++)
{
    RecordRequest("/api/users/{userId}/orders");
}
```

Trong ASP.NET Core thực tế, lấy route pattern/template từ routing metadata thay vì tự regex URL.

## Trade-off

Metrics mất khả năng drill-down theo từng user, nhưng đó không nên là nhiệm vụ của metric dimension có cardinality hữu hạn. User-specific investigation nên đi qua structured logs hoặc traces với retention/sampling/access-control phù hợp.

Không phải mọi label đều xấu: `method`, bounded status class, service, region hoặc route template thường hữu ích nếu tập giá trị được kiểm soát.

## Verify

Sửa `starter/Program.cs`, rồi chạy `./scripts/verify.ps1`.
