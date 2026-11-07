# Reference Solution — chỉ xem sau khi tự sửa

## 1. Triệu chứng
Cache hit trả ngưỡng bình thường, cache miss ném `LockRecursionException`. Đường đi có thao tác ghi không hoàn tất, khiến cấu hình mới không được tạo.

## 2. Bằng chứng
`reproduce.ps1` xác nhận `STABLE=8` qua kiểm tra nội bộ, sau đó `GetOrCreate("NEW")` phát sinh exception. Trong starter, thread đang giữ read lock trước khi yêu cầu write lock.

## 3. Root cause
`ReaderWriterLockSlim` mặc định không cho chuyển trực tiếp từ `EnterReadLock` sang `EnterWriteLock` trên cùng thread. Cách dùng này không phải một cơ chế upgrade hợp lệ.

## 4. Vì sao fix hoạt động
[Code tham khảo](ThresholdRegistry.cs) dùng pha read lock ngắn cho cache hit. Khi miss, thread nhả read lock, giành upgradeable read lock, kiểm tra lại trạng thái và chỉ sau đó vào write lock. Việc kiểm tra lại là bắt buộc vì thread khác có thể đã tạo bản ghi giữa hai pha. Write lock bảo vệ `Dictionary` và bộ đếm.

## 5. Cách verify
Sao lưu bài làm trước; sau đó chỉ để so sánh, có thể chép `solution/ThresholdRegistry.cs` sang `starter/ThresholdRegistry.cs` và chạy `./scripts/verify.ps1`. Script kiểm tra starter đang có, không chạy reference file riêng. Trên starter chưa sửa, verify phải thất bại.

## 6. Phương án khác
- Dùng một `lock` đơn giản quanh toàn bộ thao tác nếu tải thấp và code cần dễ bảo trì.
- Dùng `ConcurrentDictionary` cho cache đơn giản, nhưng cần hiểu `GetOrAdd` value factory có thể chạy nhiều lần; không đưa side effect không idempotent trực tiếp vào factory.
- Nếu tạo dữ liệu tốn I/O, cân nhắc thiết kế per-key async initialization; `ReaderWriterLockSlim` không phù hợp để giữ xuyên `await`.

## 7. Tempting / Wrong Fixes
- Bật `LockRecursionPolicy.SupportsRecursion` không biến việc nâng cấp read→write thành đúng; không giải quyết quy tắc upgrade.
- Bỏ lock rồi gọi `Dictionary.Add` trong nhiều request tạo data race.
- Chỉ catch `LockRecursionException` và trả default che lỗi nghiệp vụ.
- Chỉ thay read lock bằng upgradeable lock trên mọi request có thể giảm throughput cache hit; phải cân nhắc tải thực tế.

## 8. Production implications
Giữ critical section ngắn, đo contention và p95, tránh thực hiện network I/O khi giữ write lock. Registry in-memory chỉ bảo vệ một process; nhiều replica cần cơ chế phối hợp hoặc nguồn dữ liệu authoritative.

## 9. Trade-offs
Hai pha lock phức tạp hơn `lock` toàn cục nhưng cho phép nhiều cache hit chạy đồng thời. Với registry nhỏ và traffic thấp, lựa chọn đơn giản hơn có thể đáng tin cậy hơn. Bảo đảm exactly-once ở đây chỉ áp dụng cho thao tác tạo trong một instance.

## 10. Senior engineer cần nhận ra
Đừng chỉ dập exception: phải xác định invariant của cache miss, quyền sở hữu lock, recheck sau khi chuyển chế độ và giới hạn của concurrency guarantee. Kiểm thử đồng thời phải chạy trên đường code learner thực sự sửa.

## Tài liệu
- Microsoft Learn: [ReaderWriterLockSlim](https://learn.microsoft.com/dotnet/api/system.threading.readerwriterlockslim)
- Microsoft Learn: [Upgradeable read mode](https://learn.microsoft.com/dotnet/fundamentals/runtime-libraries/system-threading-readerwriterlockslim)
