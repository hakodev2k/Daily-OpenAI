# Reference Solution (spoiler)

**Root cause:** `System.Threading.Timer` dispatch callback lên ThreadPool mà không đợi callback trước hoàn thành. Gateway giữ callback thứ nhất, callback thứ hai cùng ghi chứng từ.

**Fix:** dùng `Interlocked.CompareExchange(ref running,1,0)` để giành quyền chạy duy nhất, return khi đã có job hoạt động. Trả quyền bằng `Volatile.Write` trong `finally` để không kẹt sau exception. Xem `Program.cs`.

**Wrong fixes:** tăng period chỉ giảm xác suất; `volatile bool` check-then-set không atomic; `lock` serialize nhưng có thể backlog và ghi trùng khi lần lượt chạy; retry vô điều kiện sau timeout có thể tạo duplicate.

**Trade-offs:** skip tick có thể mất kỳ xử lý; muốn không mất phải dùng durable queue và idempotency key. Guard chỉ bảo vệ một process, không bảo vệ hai replicas; dùng database unique constraint hoặc distributed ownership. Khi shutdown cần đợi in-flight callback riêng.

**Verification:** copy fix vào `starter/Program.cs` rồi chạy `./verify.ps1`; kỳ vọng `MAX_ACTIVE=1; POSTED=1; CYCLES=1`.
