# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Business result vẫn đúng (`Matched=1000`) nhưng equality comparison ở starter tăng tới mức rất lớn so với 2.000 products và 1.000 allowed SKUs.

## 2. Evidence

`SkuKey` đếm mỗi equality comparison. Pipeline gọi membership check một lần cho mỗi product, và collection dùng cho membership là một `List<SkuKey>`.

## 3. Root cause

`List<T>.Contains` là linear search. Khi nó nằm bên trong `Where` chạy cho từng product, một operation O(m) bị lặp lại n lần. Với hai collection cùng tăng, tổng công việc tiến gần O(n × m) thay vì tăng gần tuyến tính.

Starter còn có thể trông hợp lý vì LINQ expression ngắn và functional output đúng; vấn đề nằm ở data structure được chọn cho operation lặp lại.

## 4. Why the fix works

Reference solution materialize tập allowed SKUs thành `HashSet<SkuKey>` một lần trước khi lọc. Membership lookup trung bình gần O(1) khi hash/equality contract tốt, nên toàn bộ filter tiến gần O(n + m).

`SkuKey.GetHashCode()` dựa trên `Value`, nhất quán với `Equals`, vì vậy `HashSet` có thể dùng hash buckets đúng cách.

## 5. How to verify

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

Contract:

- `Matched=1000`
- `Comparisons<10000`
- in `VERIFY_PASS`
- exit code 0

## 6. Alternative fixes

- Dùng `Dictionary<TKey, TValue>` nếu ngoài membership còn cần lookup dữ liệu kèm theo key.
- Nếu nguồn allowed keys đã là set/indexed structure, tránh convert lặp lại mỗi invocation.
- Với database-backed `IQueryable`, để membership filter được translate thành query phù hợp thay vì kéo toàn bộ dữ liệu về memory; cần kiểm tra generated SQL và giới hạn parameter của provider.
- Với tập rất nhỏ, `List.Contains` có thể hoàn toàn đủ và đơn giản hơn. Đừng đổi data structure chỉ theo thói quen.

## 7. Wrong or misleading fixes

- **Thêm `AsParallel()` ngay lập tức:** tăng concurrency cho algorithm kém không sửa complexity và có thể làm CPU contention nặng hơn.
- **Gọi `ToList()` ở nhiều chỗ:** materialization không làm `List.Contains` trở thành lookup O(1).
- **Cache toàn bộ output vô thời hạn:** che symptom, thêm invalidation complexity và không sửa computation path.
- **Scale worker lên nhiều instance trước khi đo:** có thể tăng capacity nhưng nhân cùng một lượng công việc dư thừa trên nhiều instance.

## 8. Production implications

Membership checks dạng này thường xuất hiện ở permission filters, reconciliation, suppression lists, feature targeting và batch import. Khi input tăng 10 lần ở cả hai phía, CPU work có thể tăng gần 100 lần dù code không thay đổi và không có external dependency chậm.

## 9. Trade-offs

`HashSet` dùng thêm memory và tốn upfront construction cost. Với lookup chỉ xảy ra một lần trên tập nhỏ, linear scan có thể rẻ hơn về constant factors và đơn giản hơn. Quyết định nên dựa trên collection size, số lần lookup, equality/hash cost và lifetime của lookup structure.

## 10. What a Senior engineer should notice

Senior engineer đọc LINQ không chỉ ở mức semantics mà còn map từng operator và collection operation về cost model. Một expression ngắn có thể chứa nested work. Hãy đo algorithmic work trước khi tối ưu micro-level hoặc scale infrastructure.

## References

- Microsoft Learn — `List<T>.Contains`: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.contains
- Microsoft Learn — `HashSet<T>.Contains`: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.contains
- Microsoft Learn — LINQ: https://learn.microsoft.com/en-us/dotnet/csharp/linq/
