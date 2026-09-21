# Reference Solution

## 1. Symptoms
HTTPS request từ browser đi qua TLS-terminating proxy nhưng application thấy internal connection là HTTP và phát redirect HTTPS lần nữa.

## 2. Evidence
Transport proxy → app là HTTP trong khi forwarding evidence mô tả original client scheme là HTTPS. Redirect decision đang chạy trên connection scheme chưa được reconstruct.

## 3. Root cause
Application chưa thiết lập đúng trusted forwarded-header boundary trước logic phụ thuộc `Request.Scheme`. Vì vậy original HTTPS scheme bị mất tại TLS termination boundary.

## 4. Why fix works
Khi request đến từ trusted proxy, xử lý forwarded scheme trước redirect policy làm effective request scheme phản ánh original request. HTTPS gốc được serve; HTTP thật vẫn redirect.

## 5. How verify
Chạy `./verify.ps1`. Trường hợp proxied HTTPS phải `SERVE_REQUEST`; trường hợp HTTP không có trusted forwarding evidence phải `REDIRECT_HTTPS`.

## 6. Alternative fixes
Có thể terminate TLS trực tiếp tại application hoặc dùng hosting integration cung cấp scheme metadata theo contract khác. Dù chọn cách nào, trust boundary và middleware ordering phải rõ ràng.

## 7. Wrong/misleading fixes
Tắt HTTPS redirection chỉ che lỗi. Hard-code scheme thành HTTPS làm HTTP thật bị nhận sai. Tin `X-Forwarded-Proto` từ mọi client tạo spoofing boundary không an toàn.

## 8. Production implications
Sai scheme còn ảnh hưởng absolute URL generation, secure-cookie behavior, OAuth/OIDC redirect URI và telemetry, không chỉ redirect.

## 9. Trade-offs
Forwarded headers giữ topology linh hoạt nhưng yêu cầu cấu hình KnownProxies/KnownNetworks hoặc equivalent trust policy. TLS end-to-end giảm một số ambiguity nhưng tăng certificate và operational complexity.

## 10. What Senior engineer should notice
Đây là boundary/ordering problem giữa infrastructure và application. Cần kiểm chứng cả original request identity, trust source và middleware ordering thay vì chỉ sửa triệu chứng redirect.