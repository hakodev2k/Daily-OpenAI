# Reference Solution — chỉ đọc sau khi tự thử

## 1. Symptoms
Nhiều request cold-cache cùng một schema key trả về một cached instance đúng, nhưng số lượt compile tăng theo contention. Cache warm thường không có triệu chứng.

## 2. Evidence
Baseline khởi động đồng thời tám worker qua Barrier. Compiler tăng counter mỗi lần chạy và giả lập chi phí bằng delay. `AllWorkersShareCachedInstance=True` nhưng `FirstKeyCompileCount>1`: kết quả đúng không có nghĩa chỉ một factory chạy.

## 3. Root cause
`ConcurrentDictionary<TKey,TValue>.GetOrAdd` thread-safe với thao tác dictionary, nhưng `valueFactory` có thể được thực thi nhiều lần đồng thời cho cùng key. Một giá trị thắng và được lưu; các giá trị tạo thừa bị bỏ. Callback chạy ngoài lock nội bộ để tránh thực thi code tùy ý dưới lock.

## 4. Why the fix works
Reference `solution/SchemaCache.cs` cache `Lazy<CompiledSchema>` theo key. Nhiều caller có thể tạo nhiều Lazy wrapper, nhưng chỉ wrapper thắng được lấy qua `.Value`. `LazyThreadSafetyMode.ExecutionAndPublication` cho phép một compiler chạy cho wrapper đó; các caller khác đợi cùng kết quả. Key khác có wrapper độc lập.

## 5. How to verify
```powershell
./reproduce.ps1
./verify.ps1
dotnet run --project solution/CacheFactoryLab.Solution.csproj -- verify
```
Lệnh thứ hai kiểm tra **starter do learner sửa**, không phải solution. Kỳ vọng key đầu compile đúng một lần, key thứ hai thêm đúng một lần và mỗi key trả về một instance nhất quán.

## 6. Alternative fixes
- Per-key lock: đúng nếu quản lý vòng đời lock và không gây global contention.
- Single-flight async task cache: hợp lý với khởi tạo async, nhưng cần chính sách lỗi và cancellation rõ.
- Eager warmup: giảm cold start nhưng không giải quyết key động hay cache eviction.

## 7. Wrong / tempting fixes
- Chỉ đổi sang ConcurrentDictionary: đã dùng, không bảo đảm callback chạy một lần.
- ContainsKey trước TryAdd: tạo check-then-act race.
- Tăng cache size hoặc scale out: có thể che triệu chứng nhưng không giới hạn duplicate CPU work.
- Global lock: có thể đúng chức năng nhưng tuần tự hóa key không liên quan.

## 8. Production implications
`Lazy<T>` với ExecutionAndPublication có thể cache exception. Với lỗi transient cần chính sách evict/retry an toàn, metrics failed initialization và backoff. Key phải chứa tenant/version, tránh cardinality không giới hạn. Cache theo process không đồng nghĩa distributed exactly-once.

## 9. Trade-offs
Single-flight giảm CPU và allocation nhưng caller cùng key cùng chờ một lần compile. Per-key lock cần quản lý state; không bọc async trong Lazy rồi dùng .Result để block. Cache exceptions cần được thiết kế rõ.

## 10. What a Senior engineer should notice
Đo factory invocations, cache misses, latency, errors và số active keys; kiểm tra cold-start, eviction và scale-out. Barrier/delay chỉ giúp reproduce một process, không chứng minh behavior xuyên nhiều replica. Tách correctness của kết quả và hiệu suất của side effects.
