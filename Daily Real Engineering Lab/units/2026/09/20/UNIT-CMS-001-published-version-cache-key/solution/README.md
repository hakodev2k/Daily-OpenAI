# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Publish hoàn tất nhưng public request tiếp theo vẫn nhận version cũ; restart làm symptom biến mất.

## 2. Evidence
Source chuyển từ version 1 sang 2, trong khi cache hit tiếp tục trả object version 1 cho cùng content ID.

## 3. Root cause
Cache key chỉ biểu diễn stable content identity, không biểu diễn published representation version và cũng không có publish-triggered invalidation.

## 4. Why the fix works
Một fix nhỏ cho simulator là dùng `content:{Id}:v{Version}`. Mỗi published representation có identity riêng nên request sau publish không reuse entry của version cũ. Trong CMS thật, publish-event invalidation cũng là lựa chọn tốt nếu cache contract được quản lý tập trung.

## 5. How to verify
Sửa `starter/DeliveryCache.Get`, chạy `./verify.ps1`, rồi kiểm tra output: source và response đều version 2 sau publish.

## 6. Alternative fixes
Invalidate cache theo content ID khi nhận publish event; dùng dependency/change token nếu platform cung cấp; cache immutable representation theo version và quản lý retention.

## 7. Wrong or misleading fixes
Giảm TTL chỉ thu nhỏ cửa sổ stale chứ không tạo publish consistency. Restart application là operational workaround, không phải cache contract. Disable toàn bộ cache có thể đúng trong workload nhỏ nhưng cần đo latency/load trước khi chọn.

## 8. Production implications
CMS delivery thường có nhiều cache layer. Invalidation phải xét application cache, CDN và platform cache; mỗi layer cần ownership và consistency expectation rõ ràng.

## 9. Trade-offs
Versioned keys đơn giản và tránh invalidation race nhưng cần cleanup entries cũ. Event invalidation tiết kiệm memory hơn nhưng phụ thuộc event delivery và ordering. TTL fallback hữu ích như safety net nhưng không nên là cơ chế publish consistency duy nhất.

## 10. What a Senior engineer should notice
Cache key là data-consistency contract. Trước khi thêm cache, phải xác định identity của representation, mutation/publish boundary, invalidation owner, acceptable staleness và behavior khi invalidation event thất bại.