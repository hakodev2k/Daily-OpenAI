# Reference solution — UNIT-GETORADD-001

`ConcurrentDictionary.GetOrAdd` bảo đảm cập nhật dictionary an toàn nhưng **không** bảo đảm `valueFactory` chỉ được gọi một lần. Hai factory có thể chạy, một kết quả bị loại bỏ. Side effect bên trong factory vẫn đã xảy ra.

Reference code dùng `ConcurrentDictionary<string, Lazy<int>>`: nhiều `Lazy` có thể được tạo, nhưng chỉ `Lazy` thắng được lưu; `ExecutionAndPublication` bảo đảm value factory của đối tượng đó thực thi một lần. `GetOrAdd(...).Value` phải đọc đúng instance được trả về.

## Chạy so sánh
```powershell
dotnet run --project ./solution/TicketLab.csproj
```
Kỳ vọng `Allocations=1`, hai kết quả giống nhau, exit code 0.

## Wrong fixes
- `lock` chỉ quanh `Interlocked.Increment` không ngăn hai lần cấp mã.
- Thay `ConcurrentDictionary` bằng `Dictionary` làm mất thread safety.
- Chỉ kiểm tra `Count == 1` không đo được side effect.
- `Lazy<T>` không thay thế idempotency ở database hoặc hệ thống cấp mã bên ngoài.

## Trade-offs
`Lazy<T>` giữ exception trong cache ở chế độ `ExecutionAndPublication`. Trong production phải thiết kế eviction/retry, giới hạn cache, và idempotency key tại nơi thực hiện side effect. Không coi single-flight trong một process là bảo đảm exactly-once giữa nhiều replicas.
