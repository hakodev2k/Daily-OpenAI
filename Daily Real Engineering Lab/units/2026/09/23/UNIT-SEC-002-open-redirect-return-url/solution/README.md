# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Login thành công nhưng `Location` có thể trở thành destination do request cung cấp, kể cả destination ngoài portal.

## 2. Evidence
`reproduce.ps1` cho thấy local path và external absolute URL đều được phản chiếu vào redirect response.

## 3. Root cause
`returnUrl` là untrusted request input nhưng được dùng trực tiếp làm redirect destination. Code không enforce policy rằng post-login navigation chỉ được phép ở local application boundary.

## 4. Why the fix works
Reference implementation kiểm tra destination trước redirect. Local path hợp lệ được giữ nguyên; destination không local dùng `/` làm fallback. Trong MVC/Razor Pages production code, ưu tiên framework facilities như `Url.IsLocalUrl(...)` hoặc `LocalRedirect(...)` khi phù hợp với endpoint style.

## 5. How to verify
Chạy application với code đã sửa rồi chạy `verify.ps1`. Ba case phải pass: local path, external absolute URL và scheme-relative external URL.

## 6. Alternative fixes
- Không nhận arbitrary return URL; map một tập route key/known destination.
- Lưu intended local destination server-side trước authentication rồi dùng lại sau authentication.
- Với MVC, dùng `LocalRedirect` để framework enforce local destination tại redirect boundary.

## 7. Wrong / Tempting Fixes
- Chỉ block chuỗi bắt đầu bằng `http`: bỏ sót nhiều dạng URL khác.
- Dùng allowlist domain quá rộng khi requirement thực tế chỉ cần local navigation: tăng policy surface không cần thiết.
- Xóa hoàn toàn return navigation: tránh symptom nhưng làm hỏng UX hợp lệ.

## 8. Production implications
Open redirect thường được tận dụng để làm URL phishing đáng tin hơn vì flow bắt đầu từ domain hợp lệ. Validation nên nằm ngay tại navigation boundary và được regression-test với nhiều URL shape.

## 9. Trade-offs
Local-only policy đơn giản và an toàn khi portal không cần redirect cross-domain. Nếu business thực sự cần cross-domain SSO/navigation, cần explicit allowlist, canonicalization policy và test kỹ hơn thay vì chấp nhận arbitrary URL.

## 10. What a Senior engineer should notice
Vấn đề không nằm ở authentication correctness mà ở trust transition sau authentication. Senior engineer nên trace dữ liệu qua boundary, định nghĩa navigation contract trước, rồi chọn framework primitive phù hợp thay vì tự viết ad-hoc string checks.