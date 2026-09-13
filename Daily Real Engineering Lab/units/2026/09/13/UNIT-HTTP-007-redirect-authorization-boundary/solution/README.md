# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms

Request tới URL ban đầu có `Authorization`, server trả redirect, nhưng endpoint cuối trả `401 Unauthorized`.

## 2. Evidence

Điểm quyết định là so sánh request trước và sau redirect. Starter dùng `HttpClientHandler.AllowAutoRedirect = true`; request cuối không còn custom `Authorization` header mà caller đã gắn cho request ban đầu.

## 3. Root cause

Automatic redirect không giữ nguyên custom `Authorization` header qua redirect. Điều này tránh việc credential bị tự động mang sang một destination mới chỉ vì server trả `Location`.

## 4. Why the fix works

Reference solution tắt automatic redirect, đọc `Location`, resolve URI, kiểm tra authority của redirect target có nằm trong trust boundary đã định nghĩa hay không, rồi mới chủ động tạo request thứ hai và gắn credential.

Như vậy authentication được gửi khi cần nhưng không biến redirect thành cơ chế chuyển credential tới host tùy ý.

## 5. How to verify

Sau khi áp dụng cách xử lý tương đương vào `starter/`:

```powershell
./verify.ps1
```

Kết quả mong đợi là `FinalStatus=OK` và exit code `0`.

## 6. Alternative fixes

- Nếu API contract cho phép, gọi trực tiếp final endpoint và bỏ redirect hop.
- Dùng credential/token riêng cho destination cuối nếu hệ thống identity hỗ trợ audience/scope phù hợp.
- Đặt redirect handling trong một `DelegatingHandler` dùng chung nếu nhiều call site cần cùng policy.

## 7. Wrong / tempting fixes

- **Tự động gửi lại token tới mọi `Location`:** làm test xanh nhưng tạo credential-forwarding vulnerability.
- **Tắt authentication ở resource endpoint:** che mất lỗi contract và phá security boundary.
- **Retry request 401 nhiều lần:** không thay đổi request semantics nên chỉ tăng traffic.
- **Tăng timeout:** không liên quan tới nguyên nhân.

## 8. Production implications

Redirect là thay đổi destination. Với request có credential, redirect target phải được xem như một security decision, không chỉ là networking convenience.

Trong production cần xác định rõ:

- redirect target nào được phép;
- scheme nào được phép (`https` trong hệ thống thật);
- credential có thể được forward hay phải mint token mới;
- có cho phép redirect cross-host / cross-origin không;
- số lần redirect tối đa.

## 9. Trade-offs

Manual redirect handling thêm code và policy nhưng cho phép kiểm soát security boundary. Automatic redirect đơn giản hơn và phù hợp với request không cần custom credential hoặc khi redirect semantics đã được framework xử lý đúng với authentication mechanism đang dùng.

## 10. What a Senior engineer should notice

`401` sau redirect không nhất thiết là “token sai”. Senior engineer sẽ phân biệt authentication failure với request-shape change, kiểm tra redirect chain, headers thực tế và trust boundary trước khi thêm retry hoặc thay đổi token.
