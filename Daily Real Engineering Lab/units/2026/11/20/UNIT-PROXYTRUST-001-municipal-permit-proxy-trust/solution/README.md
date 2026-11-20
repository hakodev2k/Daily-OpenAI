# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Audit và rate-limit có thể lấy IP khác với địa chỉ nguồn mà edge thực sự nhìn thấy. Các request B/C cho thấy dữ liệu forwarding có thể do client tác động.

## Evidence
A có chain tương thích đường CDN; B bỏ qua CDN; C có tiền tố tự khai báo. Chưa có raw socket peer log ở ứng dụng nên chưa thể kết luận chính xác middleware nào đã chọn giá trị; phải bổ sung log trước khi khẳng định root cause trên production.

## Root cause hypothesis
Trust boundary của forwarding metadata không được ràng buộc chặt với các reverse proxy đã xác thực; header có thể được chấp nhận hoặc diễn giải quá nhiều hop. Đường ingress trực tiếp và config drift làm tăng rủi ro.

## One defensible fix
1. Network policy: chỉ cho phép traffic app từ ingress được quản lý; hạn chế đường trực tiếp tới ingress theo threat model.
2. Ở ingress/CDN, xác định chính sách **strip/overwrite/append** rõ ràng; không chuyển tiếp tiền tố do client tự khai báo như một identity đáng tin.
3. Trong ASP.NET Core, cấu hình `ForwardedHeadersOptions` cho đúng `KnownProxies` hoặc `KnownNetworks`, `ForwardedHeaders` và `ForwardLimit` phù hợp topology; kiểm tra IP thực tế ở runtime, không copy dải IP ví dụ vào production.
4. Đặt `UseForwardedHeaders()` ở vị trí thích hợp trước middleware cần client IP; log peer gốc, effective IP, trace ID và config revision, có kiểm soát dữ liệu cá nhân.
5. Nếu chain không hợp lệ, không dùng địa chỉ tự khai làm tín hiệu rate-limit; dùng địa chỉ peer đáng tin hoặc từ chối theo policy. Tránh tin `X-Forwarded-For` đơn thuần.
6. Tách authorization khỏi IP audit. Với rate limiting, cân nhắc principal/API key khi thích hợp và đánh giá NAT fairness.

## Verification
| Case | Expected invariant |
|---|---|
| A chuẩn | IP từ edge-validated metadata được ghi nhất quán |
| B direct | Không được chấp nhận header tự khai là client IP |
| C prefix giả | Không lấy tiền tố chưa được trusted hop xác nhận |
| Unknown proxy | Không tự động tin header |
| Header malformed | Fail closed hoặc degrade theo documented policy |
| Config drift | Tất cả replicas cùng effective trust config |
| Rollback | Có thể trở lại ingress routing ổn định mà không mở rộng trust |

Dùng test environment có thể inject request qua các route, kiểm tra app logs và ingress logs cùng trace ID. Không giả định các giá trị audit trong evidence là ground truth.

## Alternative fixes
Một edge duy nhất có thể chuẩn hóa một header nội bộ được bảo vệ bởi mTLS/network boundary. Ưu điểm: app đơn giản; nhược điểm: phụ thuộc chặt vào edge policy và rotation. Có thể thay IP-based rate limiting bằng user/tenant identity khi đã xác thực, nhưng vẫn cần bảo vệ anonymous traffic.

## Wrong / tempting fixes
- Tin phần tử đầu tiên của X-Forwarded-For: có thể là giá trị client tự khai.
- Bật trust mọi proxy hoặc tăng ForwardLimit vô hạn: mở rộng bề mặt spoofing.
- Chỉ tăng rate limit: che dấu audit attribution sai.
- Chỉ sửa một replica: tạo hành vi bất nhất.

## Production implications
Canary 1 replica, đối chiếu edge/app attribution và tỷ lệ rejected forwarding chains; kiểm tra cả đường direct. Rollback khi false-positive tăng hoặc traffic chuẩn bị chặn. Hạn chế retention và masking IP theo policy.

## Trade-offs
Fail-closed bảo vệ audit tốt nhưng có thể làm gián đoạn traffic hợp lệ khi proxy thay đổi; fail-open duy trì availability nhưng làm suy yếu security signals. Định nghĩa owner của topology, quy trình cập nhật proxy CIDR và regression test sau deployment.

## Senior engineer should notice
Không thể suy ra danh tính từ IP, nhất là sau NAT/CDN. Trust không nằm ở chuỗi ký tự header mà ở hop đã xác thực, đường mạng và chính sách biến đổi header. Sự cố cần cả evidence và ownership giữa app, network, security và SRE.

## References
- Microsoft Learn: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer
- RFC 7239 Forwarded: https://www.rfc-editor.org/rfc/rfc7239
