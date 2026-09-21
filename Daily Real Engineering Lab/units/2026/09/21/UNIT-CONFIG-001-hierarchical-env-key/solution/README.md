# Reference Solution

## 1. Symptoms
Deployment đặt một biến môi trường với giá trị 30 nhưng application vẫn đọc timeout mặc định 5 giây.

## 2. Evidence
Application đọc key phân cấp `Downstream:TimeoutSeconds`, trong khi deployment cung cấp tên biến không ánh xạ thành hierarchy mà provider cần.

## 3. Root cause
.NET environment-variable configuration provider dùng double underscore (`__`) để biểu diễn dấu phân cách `:` theo cách portable. `Downstream_TimeoutSeconds` vì vậy là một key khác, không phải `Downstream:TimeoutSeconds`.

## 4. Why fix works
Đặt deployment variable thành `Downstream__TimeoutSeconds=30` làm provider normalize tên thành đúng hierarchical key. Vì environment provider được thêm sau defaults, giá trị 30 thắng theo precedence.

## 5. How verify
Chạy `./verify.ps1`; output phải có `Downstream timeout: 30s` và `PASS`.

## 6. Alternative fixes
Có thể cung cấp cùng key qua command-line provider hoặc deployment-specific JSON nếu operational model cho phép. Với secrets, dùng secret store phù hợp thay vì source-controlled config.

## 7. Wrong/misleading fixes
Hard-code 30 trong code chỉ che mất lỗi deployment contract. Đọc `Environment.GetEnvironmentVariable` trực tiếp trong business code bỏ qua configuration abstraction và precedence. Đổi default thành 30 cũng không chứng minh override hoạt động.

## 8. Production implications
Sai naming convention có thể không làm process crash; service chạy với default nên lỗi dễ chỉ xuất hiện dưới load hoặc ở một environment cụ thể. Deployment validation nên kiểm tra effective configuration không nhạy cảm.

## 9. Trade-offs
Environment variables thuận tiện cho container/platform configuration nhưng string-based naming dễ drift. Strongly typed options và startup validation giảm rủi ro nhưng không thay thế deployment contract rõ ràng.

## 10. What Senior engineer should notice
Vấn đề nằm ở boundary giữa deployment representation và application configuration hierarchy, không phải ở parser timeout. Cần phân biệt “variable tồn tại” với “provider đã map variable vào đúng logical key”.
