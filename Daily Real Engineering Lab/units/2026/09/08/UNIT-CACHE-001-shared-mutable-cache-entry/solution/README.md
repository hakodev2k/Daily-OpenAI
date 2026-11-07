# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Request có promotion trả 80 đúng yêu cầu, nhưng request bình thường chạy sau cũng trả 80 và cached base price không còn là 100.

## 2. Evidence

Cả hai request đọc cùng cache key. Sau request đầu tiên, object còn nằm trong cache đã mang giá 80. Không có cache miss hoặc exception; sai lệch nằm ở state được chia sẻ giữa các request.

## 3. Root cause

`IMemoryCache` đang giữ reference tới một `ProductPrice` mutable. Request promotion mutate `Price` trực tiếp trên object được cache. Vì request sau nhận lại cùng cached object, nó quan sát state đã bị request trước thay đổi.

## 4. Why the fix works

Cache chỉ nên giữ base data ổn định cho use case này. Promotion là transformation theo request và phải được tính trên local value thay vì mutate cached state.

Ví dụ:

```csharp
public decimal GetPrice(int productId, bool applyPromotion)
{
    var price = cache.GetOrCreate(
        $"product-price:{productId}",
        _ => new ProductPrice { ProductId = productId, Price = 100m })!;

    return applyPromotion
        ? price.Price * 0.8m
        : price.Price;
}
```

Có thể làm mạnh hơn bằng immutable cached value:

```csharp
public sealed record ProductPrice(int ProductId, decimal Price);
```

## 5. How to verify

Chạy:

```powershell
./scripts/verify.ps1
```

Kỳ vọng: `DiscountedResponse=80`, `NormalResponse=100`, `CachedBasePrice=100`.

## 6. Alternative fixes

- Cache immutable DTO/record rồi tạo request-local result.
- Clone cached object trước khi áp dụng request-specific transformation nếu object phức tạp và mutation là cần thiết trong pipeline cục bộ.
- Cache final prices theo đầy đủ dimensions của pricing policy chỉ khi key-space, invalidation và consistency model đã được thiết kế rõ ràng.

## 7. Wrong or misleading fixes

- Tắt cache hoàn toàn: che symptom nhưng bỏ mục tiêu performance thay vì sửa ownership của state.
- Xóa cache sau mỗi request promotion: làm cache hit rate sụt và vẫn giữ mô hình state mutation khó reasoning.
- Thêm `lock` quanh mutation: serialization không sửa semantic bug; request sau vẫn đọc giá đã bị mutate.
- Tạo cache key ngẫu nhiên cho mỗi request: vô hiệu hóa cache thay vì định nghĩa đúng dữ liệu nào được chia sẻ.

## 8. Production implications

Shared mutable cache entries có thể tạo cross-request contamination khó trace, đặc biệt khi request order và concurrency thay đổi. Với distributed cache, serialization thường làm semantics khác `IMemoryCache`, nên không được giả định hai loại cache có cùng reference behavior.

## 9. Trade-offs

Immutable cache values giúp reasoning và concurrency an toàn hơn nhưng đôi khi cần allocation/copy. Cache final transformed result có thể nhanh hơn nhưng làm tăng key cardinality và invalidation complexity.

## 10. What a Senior engineer should notice

Câu hỏi cốt lõi không phải chỉ là “cache có thread-safe không”, mà là ownership và mutability của value được chia sẻ. Thread-safe cache container không tự biến object bên trong thành immutable hoặc request-isolated.
