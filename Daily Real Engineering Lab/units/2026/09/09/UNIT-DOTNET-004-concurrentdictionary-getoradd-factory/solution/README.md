# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms

Hai caller đồng thời cho cùng tenant đều nhận cùng object cuối cùng và dictionary chỉ giữ một entry, nhưng metric `CreatedClientCount` tăng thành 2. Nghĩa là invariant của collection vẫn đúng trong khi expensive initialization đã xảy ra dư thừa.

## 2. Evidence

Starter cho ba tín hiệu cần đọc cùng nhau:

- `Same instance: True`
- `Registry entries: 1`
- `Expensive initializations: 2`

Điểm quan trọng là không suy luận “một entry” thành “factory chỉ chạy một lần”.

## 3. Root cause

`ConcurrentDictionary<TKey,TValue>.GetOrAdd(key, valueFactory)` thread-safe với state của dictionary, nhưng `valueFactory` không được thực thi bên trong internal lock. Dưới contention, nhiều thread có thể cùng chạy factory cho cùng key; sau đó chỉ một candidate được publish và caller thua cuộc nhận value đã thắng.

Vì starter đặt expensive initialization trực tiếp trong `valueFactory`, side effect đó có thể xảy ra nhiều lần dù chỉ có một value được giữ.

Microsoft Learn mô tả rõ contract này trong phần Remarks của `GetOrAdd`.

## 4. Why the fix works

Reference solution đổi dictionary thành:

```csharp
ConcurrentDictionary<string, Lazy<TenantClient>>
```

`GetOrAdd` vẫn có thể tạo nhiều `Lazy<TenantClient>` candidate. Điều đó chấp nhận được vì tạo wrapper không mang expensive side effect. Chỉ wrapper thắng được trả về từ dictionary, và tất cả caller đọc `.Value` trên cùng wrapper đó.

`LazyThreadSafetyMode.ExecutionAndPublication` bảo đảm một thread thực thi value factory của `Lazy<T>` trong khi các thread khác chờ và nhận cùng value được publish.

Kết quả: callback bên ngoài có thể cạnh tranh, nhưng expensive initialization thật sự được đẩy vào một publication boundary riêng.

## 5. How to verify

Sửa trực tiếp `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Pass khi đồng thời thỏa cả ba điều kiện:

- hai caller nhận cùng instance
- registry có đúng một entry
- `CreatedClientCount == 1`

Có thể kiểm tra reference riêng bằng:

```powershell
dotnet run --project ./solution/TenantRegistrySolution.csproj --configuration Release
```

## 6. Alternative fixes

### Per-key lock / single-flight abstraction

Phù hợp khi initialization phức tạp, async, hoặc cần cancellation/error policy rõ ràng. Tránh một global lock vì các tenant độc lập không nên block nhau.

### Pre-create value rồi dùng `GetOrAdd(key, value)`

Chỉ hợp lý nếu construction rẻ và không có side effect. Nó vẫn có thể tạo nhiều object candidate, nên không giải quyết yêu cầu “expensive initialization exactly once”.

### Dedicated async lazy/single-flight primitive

Nếu factory thực sự async, tránh ép `Task<T>` qua blocking calls. Một abstraction quản lý per-key in-flight task thường rõ semantics hơn.

## 7. Wrong / Tempting Fixes

### “ConcurrentDictionary đã thread-safe nên không cần sửa”

Sai với requirement hiện tại. Thread-safety của container không bao gồm exactly-once guarantee cho arbitrary user callback.

### Global `lock` quanh toàn bộ registry

Có thể làm symptom biến mất nhưng serialize cả những tenant không liên quan, tạo bottleneck và contention không cần thiết.

### Bỏ metric vì dictionary cuối cùng vẫn đúng

Metric đang phát hiện duplicate work thật. Che metric không loại side effect hoặc resource cost.

### Retry initialization nếu thấy duplicate

Retry làm tăng duplicate work và không sửa race boundary.

## 8. Production implications

Một duplicate factory vô hại nếu callback pure và rẻ. Nó trở thành incident source khi callback:

- mở connection hoặc socket
- đăng ký subscription/callback
- tạo temp resource
- ghi external state
- consume quota
- emit billing/audit event
- giữ unmanaged resource

Vì vậy cần review side effects chứ không chỉ kiểu collection.

## 9. Trade-offs

`Lazy<T>` đơn giản và phù hợp với synchronous initialization. Tuy nhiên `ExecutionAndPublication` có semantics về exception caching: nếu initialization throw, exception có thể được cache trong `Lazy<T>`. Production code phải quyết định rõ failure có nên sticky, được evict để retry, hay chuyển sang một single-flight abstraction có lifecycle riêng.

Per-key coordination phức tạp hơn nhưng cho phép kiểm soát eviction, timeout, cancellation và retry policy tốt hơn.

## 10. What a Senior engineer should notice

- “Thread-safe” không đồng nghĩa với mọi callback surrounding API đều atomic.
- API contract phải được đọc ở boundary có side effect.
- Exactly-once execution trong concurrent/distributed code là một requirement mạnh, không nên suy ra từ tên primitive.
- Cần tách candidate creation khỏi expensive/side-effecting initialization.
- Fix tốt phải giữ concurrency giữa các key độc lập thay vì biến toàn hệ thống thành single-threaded critical section.

## Authoritative references

- Microsoft Learn — `ConcurrentDictionary<TKey,TValue>.GetOrAdd`: https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd
- Microsoft Learn — Add and remove items from `ConcurrentDictionary`: https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/how-to-add-and-remove-items
- Microsoft Learn — Lazy initialization: https://learn.microsoft.com/en-us/dotnet/framework/performance/lazy-initialization
