# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Business row vẫn tồn tại nhưng action thỉnh thoảng thất bại sau refresh.

## 2. Evidence
Target được resolve trước refresh. Refresh tạo representation mới của cùng order; target cũ không còn hợp lệ tại action time.

## 3. Root cause
Test giữ một resolved element reference qua DOM replacement. Business identity ổn định nhưng node identity không ổn định.

## 4. Why the fix works
Resolve target tại action boundary. Trong simulator, chuyển `Resolve` xuống sau `Refresh`. Trong Playwright thật, giữ `Locator` dựa trên stable user-facing identity và để Playwright resolve/action với auto-waiting semantics.

Ví dụ cho simulator:
```csharp
page.Refresh();
var approveTarget = page.Resolve("ORD-42");
return page.Approve(approveTarget);
```

## 5. How to verify
Chạy `./verify.ps1`; 20 cycle phải pass mà không dùng fixed delay.

## 6. Alternative fixes
Nếu UI có explicit completion signal, chờ signal đó rồi action qua locator. Với complex grid, dùng role/test-id ổn định kết hợp business key.

## 7. Wrong or misleading fixes
`Thread.Sleep` hoặc timeout lớn hơn chỉ thay đổi timing. Retry toàn bộ test có thể che failure. Giữ cached handle rồi retry action trên chính handle đó không thay đổi target lifetime.

## 8. Production implications
Flaky E2E tests làm giảm tín nhiệm CI, tăng rerun cost và có thể che regression thật.

## 9. Trade-offs
Stable locators cần UI contract tốt. Test-id tăng explicit test surface; role/text locators gần user behavior hơn nhưng có thể đổi theo copy/localization.

## 10. What a Senior engineer should notice
Phân biệt business identity, selector stability, DOM node lifetime và synchronization signal. Fix reliability bằng observable state/lifecycle contract thay vì delay.