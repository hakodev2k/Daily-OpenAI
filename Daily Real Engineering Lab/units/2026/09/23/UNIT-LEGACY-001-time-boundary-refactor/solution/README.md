# Reference Solution — inspect only after reproducing and attempting your own refactor

## 1. Symptoms
Business rule phụ thuộc vào thời gian hiện tại nhưng incident replay không thể buộc rule chạy tại timestamp đã ghi nhận. Boundary tests vì vậy phụ thuộc wall clock.

## 2. Evidence
`Subscription` là input tường minh, còn instant hiện tại được đọc trực tiếp bên trong service. Hai lần gọi cùng business input ở hai thời điểm khác nhau có thể cho kết quả khác.

## 3. Root cause
Business logic có hidden temporal dependency: wall clock được truy cập trực tiếp thay vì đi qua một dependency boundary có thể kiểm soát.

## 4. Reference fix
```csharp
public sealed class RenewalEligibilityService
{
    private readonly TimeProvider _timeProvider;

    public RenewalEligibilityService(TimeProvider? timeProvider = null)
        => _timeProvider = timeProvider ?? TimeProvider.System;

    public bool IsExpired(Subscription subscription)
        => _timeProvider.GetUtcNow() >= subscription.ExpiresAt;
}
```

## 5. Why the fix works
Production mặc định vẫn dùng system clock. Test/replay có thể inject một `TimeProvider` cố định, nên cùng business input và cùng instant luôn tạo cùng quyết định.

## 6. How to verify
Sửa `starter/Program.cs`, sau đó chạy `./verify.ps1`. Verifier kiểm tra trước boundary, đúng boundary và sau boundary.

## 7. Alternative fixes
Truyền `DateTimeOffset now` trực tiếp vào method có thể còn đơn giản hơn nếu “evaluation instant” thực sự là một phần của domain command. Một interface clock tự định nghĩa cũng hợp lệ trong codebase cũ, nhưng .NET 8 đã có `TimeProvider` chuẩn.

## 8. Wrong / tempting fixes
- `Thread.Sleep` để chờ clock đi qua boundary: làm test chậm và vẫn nondeterministic.
- Nới assertion bằng tolerance lớn: che symptom thay vì kiểm soát dependency.
- Mock static API bằng tooling nặng ngay lập tức: có thể cần trong legacy seam khó thay đổi, nhưng thường phức tạp hơn một dependency boundary nhỏ.
- Chuyển toàn bộ model sang local time: không giải quyết hidden dependency và còn thêm timezone ambiguity.

## 9. Production implications
Explicit time boundary hỗ trợ incident replay, deterministic tests và future migration sang scheduled/effective-time workflows. Cần thống nhất UTC semantics và tránh trộn `DateTime`, `DateTimeOffset` tùy tiện.

## 10. What a Senior engineer should notice
Refactoring legacy code an toàn không chỉ là thay API. Hãy xác định hidden input, tạo seam nhỏ, giữ behavior mặc định, rồi khóa semantics bằng boundary regression tests trước khi mở rộng thiết kế.