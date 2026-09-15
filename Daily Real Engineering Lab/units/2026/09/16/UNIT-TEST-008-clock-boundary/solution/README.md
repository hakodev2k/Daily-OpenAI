# Reference Solution — chỉ xem sau khi đã thử

## 1. Symptoms
Case sát expiration boundary có thể pass/fail theo clock của machine thay vì timestamp mà test muốn kiểm soát.

## 2. Evidence
`IsActive` nhận `observationTime` nhưng lại đọc `DateTimeOffset.Now`, nên kết quả có một hidden input.

## 3. Root cause
Business logic phụ thuộc ambient clock. Test contract và implementation không cùng sử dụng một observation time.

## 4. Why the fix works
Dùng explicit `observationTime` (hoặc inject `TimeProvider` trong production design) làm nguồn thời gian duy nhất cho decision. Ví dụ logic tối thiểu: `return observationTime < expiresAt;`.

## 5. How to verify
`verify.ps1` yêu cầu một giây trước expiry là active và đúng tại expiry là inactive trên learner-editable `starter/`.

## 6. Alternative fixes
Trong service lớn, inject .NET `TimeProvider` giúp code production vẫn tự lấy current time nhưng test có thể cung cấp clock kiểm soát được. Với pure domain function, truyền observation time trực tiếp thường đơn giản hơn.

## 7. Wrong / tempting fixes
- Thêm `Thread.Sleep` vào test: chỉ thay timing, không loại hidden dependency.
- Mở rộng tolerance tùy ý: có thể che bug boundary semantics.
- Retry flaky test: làm CI xanh ngẫu nhiên nhưng contract vẫn nondeterministic.

## 8. Production implications
Time-dependent rules xuất hiện ở expiration, retry windows, token validity, billing period và scheduling. Hidden clock làm test khó tin cậy và incident khó reproduce.

## 9. Trade-offs
Explicit timestamp tạo API rõ và pure hơn nhưng caller phải truyền thêm context. `TimeProvider` thuận tiện cho application services nhưng vẫn là dependency cần quản lý lifetime và test setup.

## 10. Senior engineer should notice
Vấn đề không chỉ là flaky test. Đây là dependency-boundary issue: mọi input ảnh hưởng business decision cần có ownership rõ, và boundary semantics (`<`, `<=`) phải được test trực tiếp.