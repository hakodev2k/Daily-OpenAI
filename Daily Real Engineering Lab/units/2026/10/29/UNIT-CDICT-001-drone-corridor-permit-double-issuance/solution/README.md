# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Hai request cùng `CORRIDOR-42` nhận cùng cached permit, nhưng fake authority đếm hai lần cấp. Cùng khóa gọi lại không tăng count; khóa khác lại tăng count.

## 2. Evidence
`reproduce.ps1` thường ghi `issued=2`, `sameCachedPermit=True`, `issuedAfterOtherKey=3`. Một entry trong cache không chứng minh external side effect chỉ xảy ra một lần.

## 3. Root cause
`ConcurrentDictionary.GetOrAdd` đảm bảo đồng bộ hóa value được lưu, nhưng `valueFactory` có thể được gọi đồng thời nhiều lần cho cùng key. Factory gọi external authority trực tiếp nên cả hai side effect xảy ra trước khi một value thắng cuộc được giữ trong dictionary.

## 4. Why the fix works
Lưu `Lazy<Permit>` vào dictionary thay vì gọi external authority trong factory của dictionary. Các factory cạnh tranh chỉ tạo các `Lazy` chưa được evaluate. Caller lấy `Lazy` thắng cuộc rồi gọi `.Value`; `ExecutionAndPublication` đảm bảo một initialization trong process cho entry đó.

Mã tham khảo: [PermitRegistry.cs](PermitRegistry.cs). Chỉ sau khi tự làm:
```powershell
Copy-Item ./solution/PermitRegistry.cs ./starter/PermitRegistry.cs -Force
./verify.ps1
```

## 5. How to verify
`verify.ps1` chạy trực tiếp `starter/`, assert `IssuedAfterRace=1`, hai caller dùng cùng permit, request lặp không tăng count, và khóa khác tạo permit mới. Đây là kiểm tra side effect và behavior, không chỉ test kiểu dữ liệu.

## 6. Alternative fixes
- Một lock per key hoặc keyed semaphore với lifecycle được quản lý: có thể rõ ràng nhưng phải giải quyết eviction và deadlock.
- Chuyển issuance thành workflow idempotent phía authority bằng request ID duy nhất, phù hợp với multi-pod và crash recovery.
- Dùng database unique constraint + durable request ledger cho yêu cầu exactly-once ở mức business; không phụ thuộc cache trong một process.

## 7. Wrong or misleading fixes
- Tin rằng `ConcurrentDictionary` tự đảm bảo factory chỉ chạy một lần: chỉ bảo đảm một value được lưu.
- Chỉ kiểm tra count cache hoặc thêm `Thread.Sleep`: không chứng minh side effect được kiểm soát.
- Đổi sang `Dictionary` thông thường: bỏ thread safety.
- Scale out: có thể làm số side effect tăng vì mỗi pod có cache riêng.

## 8. Production implications
Giải pháp `Lazy` chỉ single-flight trong cùng một instance. Khi process restart, nhiều pod cùng key, hoặc network timeout sau khi authority đã commit, vẫn cần idempotency key phía authority hoặc transactionally durable state.

## 9. Trade-offs
`Lazy<T>` có thể cache exception. Nếu external authority lỗi tạm thời, cần chính sách eviction/retry an toàn và phân biệt lỗi trước/sau khi side effect commit. Không xóa entry mù quáng rồi retry khi kết quả remote chưa rõ.

## 10. Senior engineer should notice
Concurrent collection bảo vệ cấu trúc dữ liệu chứ không mặc nhiên bảo vệ side effect trong callback. Định nghĩa rõ phạm vi bảo đảm: một value trong process, một call trong process, hay một permit trên toàn hệ thống. Đo bằng external side-effect count và kiểm thử concurrency có kiểm soát.

## References
- [ConcurrentDictionary.GetOrAdd — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd)
- [Lazy<T> — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.lazy-1)
- [LazyThreadSafetyMode — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.threading.lazythreadsafetymode)
