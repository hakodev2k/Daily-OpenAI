# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Login phát hành session cookie. `/admin/orders` nhận cookie và trả 200, còn `/reports/daily` không nhận cookie và trả 401.

## 2. Evidence
`Set-Cookie` dùng `Path=/admin`. Cookie matching cho phép cookie đó đi tới `/admin` và descendants, nhưng không tới sibling path `/reports`.

## 3. Root cause
Cookie authentication scope được cấu hình theo vị trí login thay vì theo route boundary của toàn ứng dụng cần dùng session.

## 4. Why the fix works
Trong simulator, đổi `CookiePath` thành `/` làm cả `/admin/orders` và `/reports/daily` nằm trong cookie path scope. Trong production, chọn common path hẹp nhất thực sự bao phủ các authenticated routes.

## 5. How to verify
Sửa `starter/Program.cs`, chạy `./verify.ps1`. Cả hai request phải báo `cookie-sent=True` và `status=200`.

## 6. Alternative fixes
Nếu `/reports` là ứng dụng hoặc security boundary khác, không nên mở rộng cookie. Có thể tách authentication scheme/session hoặc đặt cả hai route dưới một common prefix như `/merchant/*`.

## 7. Wrong or misleading fixes
Không sửa bằng cách bỏ authentication ở `/reports`. Không hard-code gửi `Cookie` header từ frontend JavaScript; HttpOnly cookie nên do user agent quản lý. Đổi SameSite không giải quyết path matching trong scenario same-site này.

## 8. Production implications
Cookie `Path`, `Domain`, `Secure`, `SameSite`, host và reverse-proxy routing cùng tạo nên delivery boundary. Một login 200 không chứng minh cookie sẽ xuất hiện ở mọi request sau đó.

## 9. Trade-offs
Scope rộng đơn giản hơn nhưng gửi cookie tới nhiều routes hơn. Scope hẹp giảm exposure nhưng yêu cầu route topology và authentication boundary khớp nhau.

## 10. What a Senior engineer should notice
Phân biệt authentication failure ở server với browser không gửi credential ngay từ đầu. Evidence đúng là `Set-Cookie` + request `Cookie` headers + URL boundary, không chỉ backend logs.
